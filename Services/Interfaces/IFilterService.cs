using CarDealershipManager.Models;

namespace CarDealershipManager.Services.Interfaces
{
    public interface IFilterService
    {
        Task<FilterOptions> BuildFilterOptionsAsync(int? makeId, int? modelId);
    }
}
