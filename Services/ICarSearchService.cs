using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Services
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
    }
}
