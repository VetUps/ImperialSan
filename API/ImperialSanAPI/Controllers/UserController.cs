using ImperialSanAPI.DTOs.BasketDTO;
using ImperialSanAPI.DTOs.UserDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/users
        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            using (ImperialSanContext context = new ImperialSanContext()) 
            {
                var users = context.Users.Select(u => new UserAnswerDTO
                {
                    UserId = u.UserId,
                    Email = u.UserMail,
                    Name = u.UserName,
                    Surname = u.UserSurname,
                    Patronymic = u.UserPatronymic,
                    Phone = u.UserPhone,
                    DeliveryAddress = u.DiliveryAddress,
                    Role = u.Role
                }).ToList();

                return Ok(users);
            }
        }

        // GET: api/users/*id*
        [HttpGet("{userId}")]
        public ActionResult<User> GetUser(int userId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var user = context.Users.Select(u => new UserAnswerDTO {
                    UserId = u.UserId,
                    Email = u.UserMail,
                    Name = u.UserName,
                    Surname = u.UserSurname,
                    Patronymic = u.UserPatronymic,
                    Phone = u.UserPhone,
                    DeliveryAddress = u.DiliveryAddress,
                    Role = u.Role
                }).FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                    return NotFound();

                return Ok(user);
            }
        }

        // POST: api/users/login
        [HttpPost("login")]
        public ActionResult<UserAnswerDTO> Login([FromBody] LoginUserDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                // Находим пользователя по email
                var user = context.Users.FirstOrDefault(u => u.UserMail == dto.Email);

                UsualProblemDetails loginError = new()
                {
                    Title = "Ошибка авторизации",
                    Status = StatusCodes.Status401Unauthorized,
                    Errors = new Dictionary<string, string[]>()
                        {
                               { "Login", ["Неверный email или пароль"]}
                        },
                };

                if (user == null)
                    return Unauthorized(loginError);

                if (!SecurityService.VerifyPassword(dto.Password, user.PasswordHash))
                    return Unauthorized(loginError);

                return Ok(user.UserId);
            }
        }

        // POST: api/users/register
        [HttpPost("register")]
        public ActionResult<User> RegisterUser([FromBody] RegisterUserDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (context.Users.Any(u => u.UserMail == dto.Email))
                    return Conflict("Email уже занят");

                int user_id = context.Users.ToList().Count();

                var user = new User
                {
                    UserId = user_id,
                    UserMail = dto.Email,
                    UserName = dto.Name,
                    UserSurname = dto.Surname,
                    UserPhone = dto.Phone,
                    Role = "User",
                    PasswordHash = SecurityService.HashPassword(dto.Password)
                };

                context.Users.Add(user);
                context.SaveChanges();

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
        }

        // PUT: api/users/5
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserDto dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var user = context.Users.Find(userId);
                if (user == null)
                    return NotFound();

                user.UserSurname = dto.Surname ?? user.UserSurname;
                user.UserName = dto.Name ?? user.UserName;
                user.UserPatronymic = dto.Patronymic;
                user.UserPhone = dto.Phone ?? user.UserPhone;
                user.DiliveryAddress = dto.DeliveryAddress;

                if (!string.IsNullOrEmpty(dto.NewPassword))
                {
                    user.PasswordHash = SecurityService.HashPassword(dto.NewPassword);
                }

                context.SaveChanges();

                return NoContent();
            }
        }

        // Получить корзину пользователя
        [HttpGet("{userId}/basket")]
        public  ActionResult<BasketDTO> GetBasket(int userId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .ThenInclude(p => p.Product)
                                    .FirstOrDefault(b => b.UserId == userId);

                if (basket == null)
                {
                    basket = new Basket { UserId = userId };
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
        [HttpPost("{userId}/basket/items")]
        public ActionResult<BasketDTO> AddBasketPosition(int userId, [FromBody] AddToBasketDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var basket = context.Baskets
                                    .Include(b => b.BasketPositions)
                                    .FirstOrDefault(b => b.UserId == userId);

                if (basket == null)
                {
                    basket = new Basket { UserId = userId };
                    context.Baskets.Add(basket);
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

                context.SaveChanges();

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

        // Обновить позицию
        [HttpPut("{userId}/basket/items/{positionId}")]
        public ActionResult<BasketDTO> UpdateBasketPosition(int userId, int positionId, [FromBody] UpdateBasketPositionDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var position =  context.BasketPositions
                                       .Include(p => p.Basket)
                                       .FirstOrDefault(p => p.BasketPositionId == positionId);

                if (position?.Basket?.UserId != userId)
                    return NotFound();

                position.ProductQuantity = dto.Quantity;
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
        [HttpDelete("{userId}/basket/items/{positionId}")]
        public ActionResult<BasketDTO> RemoveBasketPosition(int userId, int positionId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var position = context.BasketPositions
                                      .Include(p => p.Basket)
                                      .FirstOrDefault(p => p.BasketPositionId == positionId);

                if (position?.Basket?.UserId != userId)
                    return NotFound();

                context.BasketPositions.Remove(position);
                context.SaveChangesAsync();

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
