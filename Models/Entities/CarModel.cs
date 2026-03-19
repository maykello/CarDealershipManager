using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipManager.Models.Entities
{
    public class CarModel
    {
        [Key]
        public int CarId { get; set; }
        public int ProductionYear { get; set; }
        public int HorsePower { get; set; }
        public int Mileage { get; set; }
        public required string Vin { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public ICollection<GalleryModel>? Gallery { get; set; } = new List<GalleryModel>();
        public GenerationModel? Generation { get; set; }
        public TransmissionTypeModel TransmissionType { get; set; }
        public DrivetrainModel? Drivetrain { get; set; }
        public BodyTypeModel? BodyType { get; set; }
        public EuroClassModel? EuroClass { get; set; }
        public ColorModel? Color { get; set; }
        public required FuelTypeModel FuelType { get; set; }
        public required CarStatusModel CarStatus { get; set; }
    }
}
