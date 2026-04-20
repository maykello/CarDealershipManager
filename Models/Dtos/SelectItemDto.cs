namespace CarDealershipManager.Models.Dtos
{
    /// <summary>
    /// Generic DTO for dropdown/select list items
    /// </summary>
    public class SelectItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
