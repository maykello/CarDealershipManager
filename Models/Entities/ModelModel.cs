using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public class ModelModel
    {
        [Key]
        public int ModelId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public int? MakeId { get; set; }
        public required MakeModel Make { get; set; }
        public ICollection<GenerationModel> GenerationModel { get; set; } = new List<GenerationModel>();
    }
}
