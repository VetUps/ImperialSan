namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class BasketPositionDTO
    {
        public int BasketPositionId { get; set; }
        public int ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductTitle { get; set; } 
        public decimal ProductPrice { get; set; }
    }
}
