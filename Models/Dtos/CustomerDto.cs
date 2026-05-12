using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Models.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public CustomerType Type { get; set; } = CustomerType.Individual;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? TaxId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
    }
}
