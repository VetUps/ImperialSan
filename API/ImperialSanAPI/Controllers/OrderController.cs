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

                var orders = context.Orders.Include(o => o.OrderPositions)
                    .Where(o => o.UserId == userId)
                    .ToList();


                foreach (Order order in orders)
                {
                    ordersDTO.Add(new OrderDTO
                    {
                        OrderId = order.OrderId,
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
                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .FirstOrDefault(b => b.UserId == userId);

                if (basket == null || basket.BasketPositions.Count == 0)
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
                            Price = basket.BasketPositions.Select(bp => bp.Product.Price).Sum(),
                            UserComment = dto.UserComment
                        };

                        context.Orders.Add(newOrder);

                        foreach (BasketPosition bp in basket.BasketPositions)
                        {
                            var newOrderPosition = new OrderPosition
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = bp.ProductId,
                                ProductQuantity = bp.ProductQuantity,
                                ProductPriceInMoment = context.Products.FirstOrDefault(p => p.ProductId == bp.ProductId).Price
                            };

                            context.OrderPositions.Add(newOrderPosition);
                        }


                    }

                    catch (Exception ex)
                    {
                        UsualProblemDetails orderError = new()
                        {
                            Title = "Ошибка получения корзины",
                            Status = StatusCodes.Status500InternalServerError,
                            Errors = new Dictionary<string, string[]>()
                        {
                               { "Order", ["Не удалось сформировать заказ"]}
                        },
                        };

                        return NotFound(orderError);
                    }
                }

            }
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
