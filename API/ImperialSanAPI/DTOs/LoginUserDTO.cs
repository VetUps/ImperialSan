using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs
{
    public class LoginUserDTO
    {
        [Required(ErrorMessage = "Почта обязательна для входа")]
        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для входа")]
        public string Password { get; set; } = string.Empty;
    }
}
