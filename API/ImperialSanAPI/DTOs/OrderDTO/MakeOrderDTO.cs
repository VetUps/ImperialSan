using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class MakeOrderDTO
    {
        [Required(ErrorMessage = "Чтобы сделать заказ, ID пользователя должен быть указан")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Адресс доставки обязателен для заказа")]
        public string DiliveryAddress { get; set; }
        [Required(ErrorMessage = "Метод оплаты обязателен для заказа")]
        [AllowedValuesAttribute("Онлайн", "Наличными")]
        public string PaymentMethod { get; set; }
        public string UserComment { get; set; }
    }
}
