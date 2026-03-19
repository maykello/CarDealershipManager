using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models
{
    public class GenerationModel
    {
        [Key]
        public int GenerationId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public int? ModelId { get; set; }
        public required ModelModel Model { get; set; }
        public ICollection<CarModel> Cars { get; set; } = new List<CarModel>();
    }
}
