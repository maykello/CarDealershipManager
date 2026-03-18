using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class ColorModel
    {
        [Key]
        public int ColorId { get; set; }
        public string? ColorCode { get; set; }
        public string? Description { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
