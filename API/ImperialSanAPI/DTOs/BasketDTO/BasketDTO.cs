namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class BasketDTO
    {
        public int BasketId { get; set; }
        public List<BasketPositionDTO> Positions { get; set; } = new();
    }
}
