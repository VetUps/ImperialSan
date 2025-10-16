namespace ImperialSanAPI.DTOs.OrderDTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public List<OrderPositionDTO> Positions { get; set; } = new();
    }
}
