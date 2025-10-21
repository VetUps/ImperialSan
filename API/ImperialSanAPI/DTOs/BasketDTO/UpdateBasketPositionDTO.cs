using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class UpdateBasketPositionDTO
    {
        [Required(ErrorMessage = "Чтобы изменить позицию, нужно указать ID позиции")]
        public int BasketPositionId { get; set; }
        [Required(ErrorMessage = "Укажите колличество изменяемой позиции")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите корректное количество")]
        public int Quantity { get; set; }
    }
}
