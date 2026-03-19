using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public class BodyTypeModel
    {
        [Key]
        public int BodyTypeId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
