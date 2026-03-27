namespace CarDealershipManager.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string SearchTerm { get; set; }

        // Filter values
        public int? SelectedMakeId { get; set; }
        public int? SelectedModelId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? SelectedFuelTypeId { get; set; }
        public int? SelectedTransmissionId { get; set; }
        public int? SelectedBodyTypeId { get; set; }
        public int? SelectedColorId { get; set; }
        public int? SelectedDrivetrainId { get; set; }
        public int? SelectedEuroClassId { get; set; }
        public int? SelectedStatusId { get; set; }

        // Filter options from database
        public List<dynamic> Makes { get; set; } = new List<dynamic>();
        public List<dynamic> Models { get; set; } = new List<dynamic>();
        public List<dynamic> FuelTypes { get; set; } = new List<dynamic>();
        public List<dynamic> TransmissionTypes { get; set; } = new List<dynamic>();
        public List<dynamic> BodyTypes { get; set; } = new List<dynamic>();
        public List<dynamic> Colors { get; set; } = new List<dynamic>();
        public List<dynamic> Drivetrains { get; set; } = new List<dynamic>();
        public List<dynamic> EuroClasses { get; set; } = new List<dynamic>();
        public List<dynamic> CarStatuses { get; set; } = new List<dynamic>();

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
            SearchTerm = "";
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
