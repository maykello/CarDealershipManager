using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Services.Interfaces
{
    public interface ICarSearchService
    {
        Task<PaginatedList<CarDto>> SearchCarsAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10);

        Task<CarSearchResult> SearchCarsWithFiltersAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10);

        Task<CarDto> GetCarByIdAsync(int carId);

        Task<List<GalleryDto>> GetRandomMainPhotosAsync(int count);
    }
}
