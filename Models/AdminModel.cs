using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
