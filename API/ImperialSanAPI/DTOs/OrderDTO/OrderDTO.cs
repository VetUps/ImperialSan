namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateOnly? DateOfCreate { get; set; }

        public string? OrderStatus { get; set; }

        public string DiliveryAddres { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public float Price { get; set; }

        public string? UserComment { get; set; }
        public List<OrderPositionDTO> Positions { get; set; } = new();
    }
}
