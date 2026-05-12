using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Podaj adres email.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres email.")]
        public string Email { get; set; } = string.Empty;
    }
}
