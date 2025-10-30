using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class AddToBasketDTO
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Количество обязательно для доавбления товара в корзину")]
        [Range(-10, int.MaxValue, ErrorMessage = "Укажите корректное число товара")]
        public int Quantity { get; set; } = 1;
    }
}
