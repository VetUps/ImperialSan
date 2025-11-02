namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class OrderReceiptPositionDTO
    {
        public int? ProductId { get; set; }
        public string ProductTitle { get; set; } = "";
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Total => Price * Quantity;
    }
}
