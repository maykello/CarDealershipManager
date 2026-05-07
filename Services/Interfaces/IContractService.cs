using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Services.Interfaces
{
    public interface IContractService
    {
        Task<string> GenerateContractNumberAsync();

        Task<byte[]> GenerateContractPdfAsync(CarDto car, decimal? price = null);

        Task<ContractDto?> GetContractByIdAsync(int id);

        Task<int> CreateContractAsync(ContractDto contractDto);
    }
}
