namespace CarDealershipManager.Models.Dtos
{
    public class GenerationDto
    {
        public int GenerationId { get; set; }
        public string Name { get; set; }
        public int? ProducedSince { get; set; }
        public int? ProducedUntil { get; set; }
        public string? Description { get; set; }
        public ModelDto? Model { get; set; }
    }
}
