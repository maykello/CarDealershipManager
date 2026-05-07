namespace CarDealershipManager.Models.Dtos
{
    public class AdminDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
