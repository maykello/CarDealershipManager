using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class DrivetrainModel
    {
        [Key]
        public int DrivetrainId { get; set; }
        public string? Drivetrain { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
