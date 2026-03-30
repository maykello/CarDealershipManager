using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class CarSearchService : ICarSearchService
    {
        private readonly CarDealershipDbContext _context;

        public CarSearchService(CarDealershipDbContext context)
        {
            _context = context;
        }

        public List<CarModel> GetAllCarsWithIncludes()
        {
            return _context.Cars
                .Include(c => c.Generation)
                    .ThenInclude(g => g.Model)
                        .ThenInclude(m => m.Make)
                .Include(c => c.TransmissionType)
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.EuroClass)
                .Include(c => c.Color)
                .Include(c => c.Drivetrain)
                .Include(c => c.Gallery)
                .Include(c => c.CarStatus)
                .ToList();
        }

        public List<CarModel> ApplyFilters(List<CarModel> cars, CarFilterCriteria criteria)
        {
            return cars
                .Where(c => MatchesSearchTerm(c, criteria.SearchTerm))
                .Where(c => !criteria.MakeId.HasValue || c.Generation?.Model?.Make?.MakeId == criteria.MakeId.Value)
                .Where(c => !criteria.ModelId.HasValue || c.Generation?.Model?.ModelId == criteria.ModelId.Value)
                .Where(c => !criteria.GenerationId.HasValue || c.Generation?.GenerationId == criteria.GenerationId.Value)
                .Where(c => !criteria.MinPrice.HasValue || c.Price >= criteria.MinPrice.Value)
                .Where(c => !criteria.MaxPrice.HasValue || c.Price <= criteria.MaxPrice.Value)
                .Where(c => !criteria.MinYear.HasValue || c.ProductionYear >= criteria.MinYear.Value)
                .Where(c => !criteria.MaxYear.HasValue || c.ProductionYear <= criteria.MaxYear.Value)
                .Where(c => !criteria.FuelTypeId.HasValue || c.FuelType.FuelTypeId == criteria.FuelTypeId.Value)
                .Where(c => !criteria.TransmissionId.HasValue || c.TransmissionType.TransmissionTypeId == criteria.TransmissionId.Value)
                .Where(c => !criteria.BodyTypeId.HasValue || (c.BodyType != null && c.BodyType.BodyTypeId == criteria.BodyTypeId.Value))
                .Where(c => !criteria.ColorId.HasValue || (c.Color != null && c.Color.ColorId == criteria.ColorId.Value))
                .Where(c => !criteria.DrivetrainId.HasValue || (c.Drivetrain != null && c.Drivetrain.DrivetrainId == criteria.DrivetrainId.Value))
                .Where(c => !criteria.EuroClassId.HasValue || (c.EuroClass != null && c.EuroClass.EuroClassId == criteria.EuroClassId.Value))
                .Where(c => !criteria.StatusId.HasValue || c.CarStatus.CarStatusId == criteria.StatusId.Value)
                .ToList();
        }
        private bool MatchesSearchTerm(CarModel car, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            var searchWords = searchTerm.ToLower().Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var fullText = $"{car.Generation?.Model?.Make?.Name ?? ""} {car.Generation?.Model?.Name ?? ""} {car.Generation?.Name ?? ""}".ToLower();

            return searchWords.All(word => fullText.Contains(word));
        }
    }
}
