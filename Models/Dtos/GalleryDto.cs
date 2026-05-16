namespace CarDealershipManager.Models.Dtos
{
    public class GalleryDto
    {
        public int PhotoId { get; set; }
        public string FilePath { get; set; }
        public string? Description { get; set; }
        public string? PublicId { get; set; }
        public bool IsMain { get; set; }
        public int? CarId { get; set; }
    }
}
