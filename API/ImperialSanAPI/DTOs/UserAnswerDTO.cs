namespace ImperialSanAPI.DTOs
{
    public class UserAnswerDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Patronymic { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? DeliveryAddress { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
