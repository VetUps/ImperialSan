using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class AddProductDTO
    {
        [Required(ErrorMessage = "Название обязательно для добавления товара")]
        [MaxLength(255, ErrorMessage = "Слишком длинное название")]
        public string ProductTitle { get; set; } = null!;

        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "Цена обязательна для добавления товара")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите корректную цену")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Количестов на складе обязательно для добавления товара")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите корректное количество")]
        public int QuantityInStock { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Категория товара обязательна для добавления товара")]
        public int? CategoryId { get; set; }

        public string? BrandTitle { get; set; }

        public DateOnly? DateOfCreate { get; set; }

        public bool? IsActive { get; set; }
    }
}
