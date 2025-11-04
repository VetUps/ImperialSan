using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class ChangeStatusProductDTO
    {
        [Required(ErrorMessage = "Чтобы изменить статус товара, ID должен быть указан")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Чтобы изменить статус товара, статус должен быть указан")]
        public bool IsActive { get; set; }
    }
}
