using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Services.Interfaces
{
    public interface ICarAdminService
    {
        Task<CarDto?> GetCarByIdAsync(int id);
        Task<int> CreateCarAsync(CarDto carDto, List<IFormFile>? photos = null);
        Task UpdateCarAsync(int id, CarDto carDto, List<IFormFile>? photos = null, string? mainPhotoFilename = null);
        Task DeleteCarAsync(int id);
        Task<bool> DeletePhotoByIdAsync(int photoId);
        Task<bool> SetMainPhotoAsync(int carId, int photoId);
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
