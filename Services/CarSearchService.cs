using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class CarSearchService : ICarSearchService
    {
        private readonly CarDealershipDbContext _context;
        private readonly IFilterService _filterService;

        public CarSearchService(CarDealershipDbContext context, IFilterService filterService)
        {
            _context = context;
            _filterService = filterService;
        }
        public async Task<PaginatedList<CarModel>> SearchCarsAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _context.Cars
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
                .AsNoTracking();

            query = ApplyFiltersToQuery(query, criteria);

            var totalCount = await query.CountAsync();

            var cars = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<CarModel>(cars, totalCount, pageIndex, pageSize);
        }

        public async Task<CarSearchResult> SearchCarsWithFiltersAsync(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var paginatedList = await SearchCarsAsync(criteria, pageIndex, pageSize);
            var availableFilters = await _filterService.BuildFilterOptionsAsync(criteria.MakeId, criteria.ModelId);

            return new CarSearchResult(paginatedList, criteria, availableFilters);
        }


        private IQueryable<CarModel> ApplyFiltersToQuery(IQueryable<CarModel> query, CarFilterCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLower();
                query = query.Where(c =>
                    EF.Functions.Like(c.Generation.Model.Make.Name, $"%{searchTerm}%") ||
                    EF.Functions.Like(c.Generation.Model.Name, $"%{searchTerm}%") ||
                    EF.Functions.Like(c.Generation.Name, $"%{searchTerm}%"));
            }

            if (criteria.MakeId.HasValue)
                query = query.Where(c => c.Generation.Model.Make.MakeId == criteria.MakeId.Value);

            if (criteria.ModelId.HasValue)
                query = query.Where(c => c.Generation.Model.ModelId == criteria.ModelId.Value);

            if (criteria.GenerationId.HasValue)
                query = query.Where(c => c.Generation.GenerationId == criteria.GenerationId.Value);

            if (criteria.MinPrice.HasValue)
                query = query.Where(c => c.Price >= criteria.MinPrice.Value);

            if (criteria.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= criteria.MaxPrice.Value);

            if (criteria.MinYear.HasValue)
                query = query.Where(c => c.ProductionYear >= criteria.MinYear.Value);

            if (criteria.MaxYear.HasValue)
                query = query.Where(c => c.ProductionYear <= criteria.MaxYear.Value);

            if (criteria.FuelTypeId.HasValue)
                query = query.Where(c => c.FuelType.FuelTypeId == criteria.FuelTypeId.Value);

            if (criteria.TransmissionId.HasValue)
                query = query.Where(c => c.TransmissionType.TransmissionTypeId == criteria.TransmissionId.Value);

            if (criteria.BodyTypeId.HasValue)
                query = query.Where(c => c.BodyType != null && c.BodyType.BodyTypeId == criteria.BodyTypeId.Value);

            if (criteria.ColorId.HasValue)
                query = query.Where(c => c.Color != null && c.Color.ColorId == criteria.ColorId.Value);

            if (criteria.DrivetrainId.HasValue)
                query = query.Where(c => c.Drivetrain != null && c.Drivetrain.DrivetrainId == criteria.DrivetrainId.Value);

            if (criteria.EuroClassId.HasValue)
                query = query.Where(c => c.EuroClass != null && c.EuroClass.EuroClassId == criteria.EuroClassId.Value);

            if (criteria.StatusId.HasValue)
                query = query.Where(c => c.CarStatus.CarStatusId == criteria.StatusId.Value);

            return query;
        }
    }
}
