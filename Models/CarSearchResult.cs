using CarDealershipManager.Models.Dtos;

namespace CarDealershipManager.Models
{
    public class CarSearchResult
    {
        public PaginatedList<CarDto> Cars { get; set; }
        public CarFilterCriteria CurrentFilters { get; set; }
        public FilterOptions AvailableFilters { get; set; }

        public CarSearchResult(
            PaginatedList<CarDto> cars,
            CarFilterCriteria currentFilters,
            FilterOptions availableFilters)
        {
            Cars = cars;
            CurrentFilters = currentFilters;
            AvailableFilters = availableFilters;
        }
    }
}
