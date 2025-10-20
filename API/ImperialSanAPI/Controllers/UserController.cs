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
        // Получение всех пользователей
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

        // Получение одного пользователя
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

        // Авторизация пользователя
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

        // Регистрация пользователя
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

                return Ok(user.UserId);
            }
        }

        // Изменение данных пользователя
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
    }
}
