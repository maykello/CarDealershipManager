namespace CarDealershipManager.Models.Dtos
{
    public class ModelDto
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public MakeDto? Make { get; set; }
    }
}
