using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class FuelTypeModel
    {
        [Key]
        public int FuelTypeId { get; set; }
        public string? FuelType { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
    
}
