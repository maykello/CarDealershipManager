using System.ComponentModel.DataAnnotations;

namespace CarDealershipManager.Models.Entities
{
    public class MakeModel
    {
        [Key]
        public int MakeId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<ModelModel> Models { get; set; } = new List<ModelModel>();
    }
}
