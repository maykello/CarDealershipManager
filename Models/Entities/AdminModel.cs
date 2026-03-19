using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public class AdminModel
    {
        [Key]
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
