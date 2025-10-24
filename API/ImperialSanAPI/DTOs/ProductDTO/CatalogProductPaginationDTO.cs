namespace ImperialSanAPI.DTOs.ProductDTO
{
    public class CatalogProductPaginationDTO
    {
        public List<CatalogProductDTO> Products { get; set; }
        public int TotalProductsCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
