using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealershipManager.Models.ViewModels
{
    public enum PaymentMethod
    {
        Gotowka,
        Przelew
    }

    public class ContractGenerationViewModel
    {
        public int CarId { get; set; }
        public CarDto? Car { get; set; }
        
        public decimal Price { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Gotowka;
        public string? BankAccountNumber { get; set; }

        public bool IsNewCustomer { get; set; }

        public int? SelectedCustomerId { get; set; }
        public IEnumerable<SelectListItem>? ExistingCustomers { get; set; }

        public CustomerModel? NewCustomer { get; set; }
    }
}
