using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Models.ViewModels;

namespace CarDealershipManager.Services.Interfaces
{
    public interface IContractService
    {
        Task<string> GenerateInvoiceNumberAsync();

        Task<byte[]> GenerateInvoicePdfAsync(string invoiceNumber, CarDto car, CustomerModel buyer, CustomerModel seller, decimal price, PaymentMethod paymentMethod, string? bankAccountNumber);

        Task<ContractDto?> GetContractByIdAsync(int id);

        Task<int> CreateContractAsync(ContractDto contractDto);
    }
}
