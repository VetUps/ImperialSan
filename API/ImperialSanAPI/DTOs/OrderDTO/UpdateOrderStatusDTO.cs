using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class UpdateOrderStatusDTO
    {
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Не указан новый статус заказа")]
        [AllowedValuesAttribute("В обработке", "Собиарется", "Собран", "В пути", "Доставлен", "Отменён")]
        public string NewOrderStatus { get; set; }
    }
}
