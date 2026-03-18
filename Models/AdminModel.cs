using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
