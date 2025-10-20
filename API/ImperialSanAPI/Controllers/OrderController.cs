using ImperialSanAPI.DTOs.OrderDTO;
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
        [HttpGet("orders/{userId}")]
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
                            ProductId = op.ProductId ?? 0,
                            ProductQuantity = op.ProductQuantity ?? 0,
                            ProductPriceInMoment = op.ProductPriceInMoment,
                        }).ToList()
                    });
                }

                return Ok(ordersDTO);
            }
        }

        // Сделать заказ 
        [HttpPost("orders/make_order/{userId}")]
        public IActionResult MakeOrder(int userId, [FromBody] MakeOrderDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var basketToDelete = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .FirstOrDefault(b => b.UserId == userId);

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
                            UserId = userId,
                            DiliveryAddres = dto.DiliveryAddress,
                            PaymentMethod = dto.PaymentMethod,
                            Price = basketToDelete.BasketPositions.Select(bp => bp.Product.Price).Sum(),
                            UserComment = dto.UserComment
                        };

                        context.Orders.Add(newOrder);
                        context.SaveChanges();

                        foreach (BasketPosition bp in basketToDelete.BasketPositions)
                        {
                            var newOrderPosition = new OrderPosition
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = bp.ProductId,
                                ProductQuantity = bp.ProductQuantity,
                                ProductPriceInMoment = context.Products.FirstOrDefault(p => p.ProductId == bp.ProductId).Price
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
                            Title = "Ошибка получения корзины",
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
        [HttpPut("orders/change_status/{orderId}")]
        public IActionResult Put(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var order = context.Orders
                                   .FirstOrDefault(o => o.OrderId == dto.OrderId);

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

                order.OrderStatus = dto.NewOrderStatus;
                context.Orders.Update(order);
                context.SaveChanges();

                return Ok("Статус заказа успешно изменён!");
            }
        }

        // Отменить заказ 
        [HttpDelete("orders/abort_order/{orderId}")]
        public IActionResult Delete(int orderId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var order = context.Orders
                                   .FirstOrDefault(o => o.OrderId == orderId);

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

                order.OrderStatus = "Отменён";
                context.Orders.Update(order);
                context.SaveChanges();

                return Ok("Заказ успешно отменён!");
            }
        }
    }
}
