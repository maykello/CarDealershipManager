using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Login jest wymagany.")]
        [Display(Name = "Login")]
        [MinLength(3, ErrorMessage = "Login musi mieć co najmniej 3 znaki.")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email (opcjonalnie)")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string? Email { get; set; }
    }
}
