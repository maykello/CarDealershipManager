using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Login jest wymagany.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
