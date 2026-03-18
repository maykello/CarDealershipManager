using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class GalleryModel
    {
        [Key]
        public int PhotoId { get; set; }
        public string? FilePath { get; set; }
        public string? Description { get; set; }

        public int? CarId { get; set; }
        public CarModel Car { get; set; }
    }
    
}
