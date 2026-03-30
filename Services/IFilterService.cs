using CarDealershipManager.Models;

namespace CarDealershipManager.Services
{
    public interface IFilterService
    {
        FilterOptions BuildFilterOptions(int? makeId);
        CarFilterCriteria BuildFilterCriteria(
            string searchTerm, int? makeId, int? modelId, decimal? minPrice, decimal? maxPrice,
            int? minYear, int? maxYear, int? fuelTypeId, int? transmissionId,
            int? bodyTypeId, int? colorId, int? drivetrainId, int? euroClassId, int? statusId);
    }
}
