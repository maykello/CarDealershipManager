using CarDealershipManager.Models;

namespace CarDealershipManager.Services
{
    public interface IFilterService
    {
        Task<FilterOptions> BuildFilterOptionsAsync(int? makeId, int? modelId);
    }
}
