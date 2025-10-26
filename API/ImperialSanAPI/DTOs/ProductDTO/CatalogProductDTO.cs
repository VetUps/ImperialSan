using ImperialSanAPI.Models;

namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class CatalogProductDTO
    {
        public int ProductId { get; set; }

        public string? ProductTitle { get; set; }

        public string? ProductDescription { get; set; }

        public float Price { get; set; }

        public int QuantityInStock { get; set; }

        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public string? BrandTitle { get; set; }

        public DateOnly? DateOfCreate { get; set; }

        public bool? IsActive { get; set; }

        public string? CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
