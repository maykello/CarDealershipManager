using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class BodyTypeModel
    {
        [Key]
        public int BodyTypeId { get; set; }
        public string? BodyName { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
