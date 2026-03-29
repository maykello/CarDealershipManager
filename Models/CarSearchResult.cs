namespace CarDealershipManager.Models
{
    public class CarSearchResult
    {
        public PaginatedList<Entities.CarModel> Cars { get; set; }
        public CarFilterCriteria CurrentFilters { get; set; }
        public FilterOptions AvailableFilters { get; set; }
        public int CurrentPageIndex { get; set; }

        public CarSearchResult(
            PaginatedList<Entities.CarModel> cars,
            CarFilterCriteria currentFilters,
            FilterOptions availableFilters,
            int pageIndex)
        {
            Cars = cars ?? throw new ArgumentNullException(nameof(cars));
            CurrentFilters = currentFilters ?? throw new ArgumentNullException(nameof(currentFilters));
            AvailableFilters = availableFilters ?? throw new ArgumentNullException(nameof(availableFilters));
            CurrentPageIndex = pageIndex;
        }
    }
}
