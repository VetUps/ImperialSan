namespace ImperialSanAPI.DTOs.BasketDTO
{
    public class AddToBasketDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
