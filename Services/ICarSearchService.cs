using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services
{
    public interface ICarSearchService
    {
        List<CarModel> GetAllCarsWithIncludes();

        List<CarModel> ApplyFilters(List<CarModel> cars, CarFilterCriteria criteria);
    }
}
