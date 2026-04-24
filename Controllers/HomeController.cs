using CarDealershipManager.Models;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarDealershipManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarSearchService _carSearchService;
        private readonly IFilterService _filterService;

        public HomeController(
            ICarSearchService carSearchService,
            IFilterService filterService)

        {
            _carSearchService = carSearchService;
            _filterService = filterService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Cars(
            CarFilterCriteria criteria,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var searchResult = await _carSearchService.SearchCarsWithFiltersAsync(criteria, pageIndex, pageSize);
            return View(searchResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _carSearchService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound();
            return View(car);
        }

        [HttpGet]
        public async Task<IActionResult> GetModelsByMake(int? makeId)
        {
            var models = await _filterService.BuildFilterOptionsAsync(makeId, null);
            return Json(models.Models);
        }
        [HttpGet]
        public async Task<IActionResult> GetGenerationsByModel(int? modelId)
        {
            var generations = await _filterService.BuildFilterOptionsAsync(null, modelId);
            return Json(generations.Generations);
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
