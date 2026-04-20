using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Services
{
    public interface ICarAdminService
    {
        Task<CarDto?> GetCarByIdAsync(int id);
        Task<int> CreateCarAsync(CarDto carDto);
        Task UpdateCarAsync(int id, CarDto carDto);
        Task DeleteCarAsync(int id);
        Task<bool> CarExistsAsync(int id);
        Task<List<SelectItemDto>> GetMakesAsync();
        Task<List<SelectItemDto>> GetModelsAsync();
        Task<List<SelectItemDto>> GetGenerationsAsync();
        Task<List<SelectItemDto>> GetTransmissionTypesAsync();
        Task<List<SelectItemDto>> GetDrivetrainsAsync();
        Task<List<SelectItemDto>> GetBodyTypesAsync();
        Task<List<SelectItemDto>> GetEuroClassesAsync();
        Task<List<SelectItemDto>> GetColorsAsync();
        Task<List<SelectItemDto>> GetFuelTypesAsync();
        Task<List<SelectItemDto>> GetCarStatusesAsync();
        Task<List<SelectItemDto>> GetModelsByMakeAsync(int makeId);
        Task<List<SelectItemDto>> GetGenerationsByModelAsync(int modelId);
    }
}
