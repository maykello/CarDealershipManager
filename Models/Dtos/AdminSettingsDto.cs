using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Dtos
{
    public class AdminSettingsDto
    {
        [Display(Name = "Adres Email")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Aktualne hasło jest wymagane.")]
        [Display(Name = "Aktualne hasło (wymagane do zatwierdzenia zmian)")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Display(Name = "Nowe hasło (zostaw puste aby nie zmieniać)")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Nowe hasło musi mieć co najmniej 5 znaków.")]
        public string? NewPassword { get; set; }

        [Display(Name = "Potwierdź nowe hasło")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Podane hasła nie są identyczne.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
