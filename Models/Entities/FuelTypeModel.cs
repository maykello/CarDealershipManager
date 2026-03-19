using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public class FuelTypeModel
    {
        [Key]
        public int FuelTypeId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
    
}
