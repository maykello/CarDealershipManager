using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public enum CustomerType
    {
        [Display(Name = "Osoba fizyczna")]
        Individual,
        
        [Display(Name = "Firma")]
        Company
    }

    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public CustomerType Type { get; set; } = CustomerType.Individual;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? TaxId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public string? PostalCode { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public ICollection<ContractModel> Contracts { get; set; } = new List<ContractModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
    }
}