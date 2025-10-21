using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class DeleteBasketPositionDTO
    {
        [Required(ErrorMessage = "Чтобы удалить позицию, ID позиции нужно указать")]
        public int BasketPositionId { get; set; }
    }
}
