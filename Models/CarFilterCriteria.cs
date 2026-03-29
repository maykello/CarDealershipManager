namespace CarDealershipManager.Models
{
    public class CarFilterCriteria
    {
        public string SearchTerm { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? MakeId { get; set; }
        public int? ModelId { get; set; }
        public int? FuelTypeId { get; set; }
        public int? TransmissionId { get; set; }
        public int? BodyTypeId { get; set; }
        public int? ColorId { get; set; }
        public int? DrivetrainId { get; set; }
        public int? EuroClassId { get; set; }
        public int? StatusId { get; set; }
    }
}
