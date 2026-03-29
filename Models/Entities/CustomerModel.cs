using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public enum CustomerType
    {
        Individual,
        Company
    }

    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public CustomerType Type { get; set; } = CustomerType.Individual;
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(150)]
        public string? CompanyName { get; set; }
        [MaxLength(50)]
        public string? NationalIdNumber { get; set; }
        [MaxLength(50)]
        public string? TaxId { get; set; }
        [MaxLength(50)]
        public string? DocumentNumber { get; set; }
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }
        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }
        [MaxLength(100)]
        public required string Country { get; set; }
        [MaxLength(100)]
        public required string City { get; set; }
        [MaxLength(20)]
        public string? PostalCode { get; set; }
        [MaxLength(150)]
        public string? AddressLine1 { get; set; }
        [MaxLength(150)]
        public string? AddressLine2 { get; set; }
        public ICollection<ContractModel> Contracts { get; set; } = new List<ContractModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
    }
}