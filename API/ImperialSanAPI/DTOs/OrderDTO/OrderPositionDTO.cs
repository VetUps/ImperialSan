namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class OrderPositionDTO
    {
        public int OrderPositionId { get; set; }
        public int ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public float ProductPriceInMoment { get; set; }
    }
}
