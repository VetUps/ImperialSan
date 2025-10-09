using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.UserDTO
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Почта обязательна для регистрации")]
        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        [MaxLength(100, ErrorMessage = "Почта слишком длиная")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для регистрации")]
        [MinLength(6, ErrorMessage = "Пароль должен состоять минимум из 6 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Повторить пароль обязательно для регистрации")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string RepeatPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна для регистрации")]
        [MaxLength(100, ErrorMessage = "Фамилия слишком длиная")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно для регистрации")]
        [MaxLength(100, ErrorMessage = "Имя слишком длинное")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Отчество слшиком длинное")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Телефон обязателен для регистрации")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Неверный формат номера (ровно 11 цифр)")]
        public string Phone { get; set; } = string.Empty;

        public string? DeliveryAddress { get; set; }
    }
}
