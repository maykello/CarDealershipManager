using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class CarStatusModel
    {
        [Key]
        public int CarStatusId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
