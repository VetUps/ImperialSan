using ImperialSanAPI.DTOs.UserDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;

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

        // Получение пользователя
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
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения пользователя",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "User", ["Такого пользователя не существует"]}
                        },
                    });
                }

                return Ok(user);
            }
        }

        // Авторизация пользователя
        [HttpPost("login")]
        public ActionResult<UserAnswerDTO> Login([FromBody] LoginUserDTO loginUserDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                // Находим пользователя по email
                var user = context.Users.FirstOrDefault(u => u.UserMail == loginUserDto.Email);

                if (user == null)
                    return Unauthorized(new UsualProblemDetails
                    {
                        Title = "Ошибка авторизации",
                        Status = StatusCodes.Status401Unauthorized,
                        Errors = new Dictionary<string, string[]>()
                        {
                            { "Login", ["Неверный email или пароль"]}
                        },
                    });

                if (!SecurityService.VerifyPassword(loginUserDto.Password, user.PasswordHash))
                    return Unauthorized(new UsualProblemDetails
                    {
                        Title = "Ошибка авторизации",
                        Status = StatusCodes.Status401Unauthorized,
                        Errors = new Dictionary<string, string[]>()
                        {
                            { "Login", ["Неверный email или пароль"]}
                        },
                    });

                return Ok(user.UserId);
            }
        }

        // Регистрация пользователя
        [HttpPost("register")]
        public ActionResult<int> RegisterUser([FromBody] RegisterUserDTO registerUserDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (context.Users.Any(u => u.UserMail == registerUserDto.Email))
                {
                    return Conflict(new UsualProblemDetails
                    {
                        Title = "Ошибка регистрации",
                        Status = StatusCodes.Status401Unauthorized,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Email", ["Такой email уже занят"]}
                        },
                    });
                }

                int user_id = context.Users.ToList().Count();

                var user = new User
                {
                    UserId = user_id,
                    UserMail = registerUserDto.Email,
                    UserName = registerUserDto.Name,
                    UserSurname = registerUserDto.Surname,
                    UserPatronymic = registerUserDto.Patronymic,
                    UserPhone = registerUserDto.Phone,
                    Role = "User",
                    PasswordHash = SecurityService.HashPassword(registerUserDto.Password)
                };

                context.Users.Add(user);
                context.SaveChanges();

                return Ok(user.UserId);
            }
        }

        // Изменение данных пользователя
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var user = context.Users.Find(updateUserDto.UserId);
                if (user == null)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения пользователя",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "User", ["Такого пользователя не существует"]}
                        },
                    });
                }

                user.UserSurname = updateUserDto.Surname;
                user.UserName = updateUserDto.Name;
                user.UserPatronymic = updateUserDto.Patronymic;
                user.UserPhone = updateUserDto.Phone;
                user.DiliveryAddress = updateUserDto.DeliveryAddress;

                if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
                {
                    user.PasswordHash = SecurityService.HashPassword(updateUserDto.NewPassword);
                }

                context.SaveChanges();

                return Ok();
            }
        }
    }
}
