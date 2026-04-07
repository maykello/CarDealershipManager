using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services
{
    public interface ICarSearchService
    {
        Task<PaginatedList<CarModel>> SearchCarsAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10);

        Task<CarSearchResult> SearchCarsWithFiltersAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10);
    }
}
