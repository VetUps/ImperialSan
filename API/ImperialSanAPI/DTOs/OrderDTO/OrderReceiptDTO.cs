namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class OrderReceiptDTO
    {
        public int OrderId { get; set; }
        public DateOnly? DateOfCreate { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public float TotalPrice { get; set; }
        public List<OrderReceiptPositionDTO> Positions { get; set; } = new();
    }
}
