namespace ImperialSanAPI.DTOs
{
    public class UpdateUserDto
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? NewPassword { get; set; }
    }
}
