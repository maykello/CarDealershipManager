using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class GalleryModel
    {
        [Key]
        public int PhotoId { get; set; }
        public required string FilePath { get; set; }
        public string? Description { get; set; }

        public int? CarId { get; set; }
        public required CarModel Car { get; set; }
    }
    
}
