using ImperialSanAPI.DTOs.BasketDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        // Получить корзину пользователя
        [HttpGet("{userId}")]
        public ActionResult<BasketDTO> GetBasket(int userId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .FirstOrDefault(b => b.UserId == userId);

                if (basket == null)
                {
                    basket = new Basket { 
                        UserId = userId 
                    };
                    context.Baskets.Add(basket);
                    context.SaveChanges();
                }

                var basketDto = new BasketDTO
                {
                    BasketId = basket.BasketId,
                    Positions = basket.BasketPositions.Select(p => new BasketPositionDTO
                    {
                        BasketPositionId = p.BasketPositionId,
                        ProductId = p.ProductId ?? 0,
                        ProductQuantity = p.ProductQuantity ?? 0
                    }).ToList()
                };

                return Ok(basketDto);
            }
        }

        // Добавить товар в корзину
        [HttpPost("{userId}/add_position")]
        public ActionResult<BasketDTO> AddBasketPosition(int userId, [FromBody] AddToBasketDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == dto.ProductId);
                if (product == null)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    });
                }

                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .FirstOrDefault(b => b.UserId == userId);

                if (basket == null)
                {
                    basket = new Basket
                    {
                        BasketId = context.Baskets.Count(),
                        UserId = userId
                    };
                    context.Baskets.Add(basket);
                    context.SaveChanges();
                }

                var existing = basket.BasketPositions.FirstOrDefault(p => p.ProductId == dto.ProductId);
                if (existing != null)
                {
                    existing.ProductQuantity += dto.Quantity;
                }
                else
                {
                    basket.BasketPositions.Add(new BasketPosition
                    {
                        ProductId = dto.ProductId,
                        ProductQuantity = dto.Quantity
                    });
                }

                //product.QuantityInStock -= dto.Quantity;
                context.SaveChanges();

                var basketDto = new BasketDTO
                {
                    BasketId = basket.BasketId,
                    Positions = basket.BasketPositions.Select(p => new BasketPositionDTO
                    {
                        BasketPositionId = p.BasketPositionId,
                        ProductId = p.ProductId ?? 0,
                        ProductQuantity = p.ProductQuantity ?? 0,
                    }).ToList()
                };

                return Ok(basketDto);
            }
        }

        // Обновить позицию
        [HttpPut("{userId}/update_position")]
        public ActionResult<BasketDTO> UpdateBasketPosition(int userId, [FromBody] UpdateBasketPositionDTO updateBasketPositionDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var position = context.BasketPositions
                                       .Include(p => p.Basket)
                                       .FirstOrDefault(p => p.BasketPositionId == updateBasketPositionDto.BasketPositionId);

                if (position?.Basket?.UserId != userId)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения позиции",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такой позиции не существует в корзине"]}
                        },
                    });
                }

                position.ProductQuantity = updateBasketPositionDto.Quantity;
                context.SaveChanges();

                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .First(b => b.UserId == userId);

                var basketDto = new BasketDTO
                {
                    BasketId = basket.BasketId,
                    Positions = basket.BasketPositions.Select(p => new BasketPositionDTO
                    {
                        BasketPositionId = p.BasketPositionId,
                        ProductId = p.ProductId ?? 0,
                        ProductQuantity = p.ProductQuantity ?? 0
                    }).ToList()
                };

                return Ok(basketDto);
            }
        }

        // Удалить позицию
        [HttpDelete("{userId}/delete_position")]
        public ActionResult<BasketDTO> RemoveBasketPosition(int userId, [FromBody] DeleteBasketPositionDTO deleteBasketPositionDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var position = context.BasketPositions
                                      .Include(p => p.Basket)
                                      .FirstOrDefault(p => p.BasketPositionId == deleteBasketPositionDto.BasketPositionId);

                if (position?.Basket?.UserId != userId)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка удаления товара",
                        Status = StatusCodes.Status401Unauthorized,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует в корзине"]}
                        },
                    });
                }

                context.BasketPositions.Remove(position);
                context.SaveChanges();

                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .First(b => b.UserId == userId);

                var basketDto = new BasketDTO
                {
                    BasketId = basket.BasketId,
                    Positions = basket.BasketPositions.Select(p => new BasketPositionDTO
                    {
                        BasketPositionId = p.BasketPositionId,
                        ProductId = p.ProductId ?? 0,
                        ProductQuantity = p.ProductQuantity ?? 0
                    }).ToList()
                };

                return Ok(basketDto);
            }
        }
    }
}
