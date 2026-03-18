using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class EuroClassModel
    {
        [Key]
        public int EuroClassId { get; set; }
        public string? EuroClassName { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
