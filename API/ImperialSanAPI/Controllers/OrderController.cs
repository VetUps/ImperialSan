﻿using ImperialSanAPI.DTOs.OrderDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // Получить заказы пользователя
        [HttpGet("{userId}")]
        public ActionResult<List<OrderDTO>> GetOrders(int userId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                List<OrderDTO> ordersDTO = new List<OrderDTO>();

                var orders = context.Orders
                                    .Include(o => o.OrderPositions)
                                    .Where(o => o.UserId == userId)
                                    .ToList();


                foreach (Order order in orders)
                {
                    ordersDTO.Add(new OrderDTO
                    {
                        OrderId = order.OrderId,
                        DateOfCreate = order.DateOfCreate,
                        OrderStatus = order.OrderStatus,
                        DiliveryAddres = order.DiliveryAddres,
                        PaymentMethod = order.PaymentMethod,
                        Price = order.Price,
                        UserComment = order.UserComment,
                        Positions = order.OrderPositions.Select(op => new OrderPositionDTO
                        {
                            OrderPositionId = op.OrderPositionId,
                            ProductId = op.ProductId,
                            ProductQuantity = op.ProductQuantity,
                            ProductPriceInMoment = op.ProductPriceInMoment,
                        }).ToList()
                    });
                }

                return Ok(ordersDTO);
            }
        }

        // Сделать заказ 
        [HttpPost("make_order")]
        public IActionResult MakeOrder([FromBody] MakeOrderDTO makeOrderDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var basketToDelete = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .FirstOrDefault(b => b.UserId == makeOrderDto.UserId);

                if (basketToDelete == null || basketToDelete.BasketPositions.Count == 0)
                {
                    UsualProblemDetails orderError = new()
                    {
                        Title = "Ошибка получения корзины",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Basket", ["Коризна пуста"]}
                        },
                    };

                    return NotFound(orderError);
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var newOrder = new Order
                        {
                            UserId = makeOrderDto.UserId,
                            DiliveryAddres = makeOrderDto.DiliveryAddress,
                            PaymentMethod = makeOrderDto.PaymentMethod,
                            Price = basketToDelete.BasketPositions.Select(bp => bp.Product.Price).Sum(),
                            UserComment = makeOrderDto.UserComment
                        };

                        context.Orders.Add(newOrder);
                        context.SaveChanges();

                        foreach (BasketPosition bp in basketToDelete.BasketPositions)
                        {
                            var product = context.Products.FirstOrDefault(p => p.ProductId == bp.ProductId);
                            if (product == null)
                            {
                                throw new Exception($"Товара #{bp.ProductId} больше не существует");
                            }

                            if (bp.ProductQuantity > product.QuantityInStock)
                            {
                                throw new Exception($"Максимум можно заказать {product.QuantityInStock} шт. товара #{product.ProductId} {product.ProductTitle}");
                            }

                            var newOrderPosition = new OrderPosition
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = bp.ProductId,
                                ProductQuantity = bp.ProductQuantity,
                                ProductPriceInMoment = product.Price
                            };

                            context.OrderPositions.Add(newOrderPosition);
                            context.SaveChanges();
                        }

                        if (basketToDelete != null)
                        {
                            context.Baskets.Remove(basketToDelete);
                        }

                        context.SaveChanges();
                        transaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        UsualProblemDetails orderError = new()
                        {
                            Title = "Ошибка формирования заказа",
                            Status = StatusCodes.Status500InternalServerError,
                            Errors = new Dictionary<string, string[]>()
                            {
                                   { "Order", [$"Не удалось сформировать заказ: {ex.Message}"]}
                            },
                        };

                        return BadRequest(orderError);
                    }
                }

                return Ok("Заказ успешно создан!");

            }
        }

        // Изменить статус заказа 
        [HttpPut("change_status")]
        public IActionResult Put([FromBody] UpdateOrderStatusDTO updateOrderStatusDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var order = context.Orders
                                   .FirstOrDefault(o => o.OrderId == updateOrderStatusDto.OrderId);

                if (order == null)
                {
                    UsualProblemDetails orderError = new()
                    {
                        Title = "Ошибка получения заказа",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                            {
                                   { "Order", ["Заказ не найден"]}
                            },
                    };

                    return NotFound(orderError);
                }

                order.OrderStatus = updateOrderStatusDto.NewOrderStatus;
                context.Orders.Update(order);
                context.SaveChanges();

                return Ok("Статус заказа успешно изменён!");
            }
        }

        // Отменить заказ 
        [HttpDelete("abort_order")]
        public IActionResult Delete([FromBody] AbortOrderDTO abortOrderDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var order = context.Orders
                                   .FirstOrDefault(o => o.OrderId == abortOrderDto.OrderId);

                if (order == null)
                {
                    UsualProblemDetails orderError = new()
                    {
                        Title = "Ошибка получения заказа",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                            {
                                   { "Order", ["Заказ не найден"]}
                            },
                    };

                    return NotFound(orderError);
                }

                if (order.OrderStatus != "В обработке")
                {
                    UsualProblemDetails orderError = new()
                    {
                        Title = "Ошибка отмены заказа",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                            {
                                   { "Order", [$"Заказ нельзя отменить, т.к. он уже в стадии \"{order.OrderStatus}\""]}
                            },
                    };

                    return NotFound(orderError);
                }

                order.OrderStatus = "Отменён";
                context.Orders.Update(order);
                context.SaveChanges();

                return Ok("Заказ успешно отменён!");
            }
        }
    }
}
