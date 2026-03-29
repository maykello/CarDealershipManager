using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarDealershipManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarSearchService _carSearchService;
        private readonly IFilterService _filterService;
        private readonly CarDealershipDbContext _context;

        public HomeController(
            ICarSearchService carSearchService, 
            IFilterService filterService,
            CarDealershipDbContext context)
        {
            _carSearchService = carSearchService;
            _filterService = filterService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cars(
            int pageIndex = 1,
            int pageSize = 10,
            string searchTerm = "",
            int? makeId = null,
            int? modelId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minYear = null,
            int? maxYear = null,
            int? fuelTypeId = null,
            int? transmissionId = null,
            int? bodyTypeId = null,
            int? colorId = null,
            int? drivetrainId = null,
            int? euroClassId = null,
            int? statusId = null)
        {
            pageSize = PaginationValidator.ValidatePageSize(pageSize);

            var allCars = _carSearchService.GetAllCarsWithIncludes();
            var filterCriteria = _filterService.BuildFilterCriteria(
                searchTerm, makeId, modelId, minPrice, maxPrice,
                minYear, maxYear, fuelTypeId, transmissionId,
                bodyTypeId, colorId, drivetrainId, euroClassId, statusId);

            allCars = _carSearchService.ApplyFilters(allCars, filterCriteria);

            var totalCount = allCars.Count;
            var paginatedCars = allCars
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedList = new PaginatedList<CarModel>(paginatedCars, totalCount, pageIndex, pageSize);
            var availableFilters = _filterService.BuildFilterOptions(makeId);
            var searchResult = new CarSearchResult(paginatedList, filterCriteria, availableFilters, pageIndex);

            return View(searchResult);
        }

        public IActionResult GetModelsByMake(int? makeId)
        {
            List<dynamic> models = new List<dynamic>();

            if (makeId.HasValue)
            {
                models = _context.Models
                    .Where(m => m.Make.MakeId == makeId.Value)
                    .Select(m => new { m.ModelId, m.Name })
                    .Cast<dynamic>()
                    .ToList();
            }
            else
            {
                models = _context.Models
                    .Select(m => new { m.ModelId, m.Name })
                    .Cast<dynamic>()
                    .ToList();
            }

            return Json(models);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
