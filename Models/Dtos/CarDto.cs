namespace CarDealershipManager.Models.Dtos
{
    public class CarDto
    {
        public int CarId { get; set; }
        public int ProductionYear { get; set; }
        public int HorsePower { get; set; }
        public int Mileage { get; set; }
        public string Vin { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }

        public GenerationDto? Generation { get; set; }
        public TransmissionTypeDto? TransmissionType { get; set; }
        public DrivetrainDto? Drivetrain { get; set; }
        public BodyTypeDto? BodyType { get; set; }
        public EuroClassDto? EuroClass { get; set; }
        public ColorDto? Color { get; set; }
        public FuelTypeDto? FuelType { get; set; }
        public CarStatusDto? CarStatus { get; set; }
        public ICollection<GalleryDto>? Gallery { get; set; } = new List<GalleryDto>();
    }
}
