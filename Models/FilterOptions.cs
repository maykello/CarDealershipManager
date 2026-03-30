namespace CarDealershipManager.Models
{

    public class FilterOptions
    {
        public IReadOnlyList<dynamic> Makes { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> Models { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> Generations { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> FuelTypes { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> TransmissionTypes { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> BodyTypes { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> Colors { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> Drivetrains { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> EuroClasses { get; set; } = new List<dynamic>().AsReadOnly();
        public IReadOnlyList<dynamic> CarStatuses { get; set; } = new List<dynamic>().AsReadOnly();
    }
}
