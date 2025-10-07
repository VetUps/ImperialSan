using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class UpdateProductDTO
    {
        [Required(ErrorMessage = "Название нельзя имзенить на пустое")]
        [MaxLength(255, ErrorMessage = "Слишком длинное название")]
        public string ProductTitle { get; set; } = null!;

        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "Цену нельзя убрать")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите корректную цену")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Количество товара на складе должно быть указано")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите корректное количество")]
        public int QuantityInStock { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Категория товара должна быть указана")]
        public int? CategoryId { get; set; }

        public string? BrandTitle { get; set; }
    }
}
