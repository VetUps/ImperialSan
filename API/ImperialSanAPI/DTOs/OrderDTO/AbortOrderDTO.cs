using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class AbortOrderDTO
    {
        [Required(ErrorMessage = "Чтобы отменить заказ, ID заказа должен быть указан")]
        public int OrderId { get; set; }
    }
}
