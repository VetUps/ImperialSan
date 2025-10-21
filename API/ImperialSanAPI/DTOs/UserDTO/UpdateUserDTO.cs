using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.UserDTO
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Чтобы изменить пользователя, ID должен быть указан")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Фамилия не может быть пустой")]
        [MaxLength(100, ErrorMessage = "Фамилия слишком длиная")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Имя не может быть пустым")]
        [MaxLength(100, ErrorMessage = "Имя слишком длинное")]
        public string? Name { get; set; }

        [MaxLength(100, ErrorMessage = "Отчество слшиком длинное")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Телефон не может быть пустым")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Неверный формат номера (ровно 11 цифр)")]
        public string? Phone { get; set; }

        public string? DeliveryAddress { get; set; }
        public string? NewPassword { get; set; }
    }
}
