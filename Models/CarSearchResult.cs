namespace CarDealershipManager.Models
{
    public class CarSearchResult
    {
        public PaginatedList<Entities.CarModel> Cars { get; set; }
        public CarFilterCriteria CurrentFilters { get; set; }
        public FilterOptions AvailableFilters { get; set; }

        public CarSearchResult(
            PaginatedList<Entities.CarModel> cars,
            CarFilterCriteria currentFilters,
            FilterOptions availableFilters)
        {
            Cars = cars;
            CurrentFilters = currentFilters;
            AvailableFilters = availableFilters;
        }
    }
}
