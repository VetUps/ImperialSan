using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class DeleteProductDTO
    {
        [Required(ErrorMessage = "Чтобы удалить товар, ID должен быть указан")]
        public int ProductId { get; set; }
    }
}
