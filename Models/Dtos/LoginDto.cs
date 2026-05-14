using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Login lub email jest wymagany.")]
        [Display(Name = "Login lub Email")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
