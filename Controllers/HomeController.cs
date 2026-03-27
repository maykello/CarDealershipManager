using CarDealershipManager.Models;
using CarDealershipManager.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarDealershipManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarDealershipDbContext _context;

        public HomeController(CarDealershipDbContext context)
        {
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
            // Validate pageSize - allow only 10, 20, 50
            if (pageSize != 10 && pageSize != 20 && pageSize != 50)
            {
                pageSize = 10;
            }

            var allCars = _context.Cars
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

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                allCars = allCars.Where(c =>
                    (c.Generation?.Model?.Make?.Name ?? "").ToLower().Contains(lowerSearchTerm) ||
                    (c.Generation?.Model?.Name ?? "").ToLower().Contains(lowerSearchTerm) ||
                    (c.Generation?.Name ?? "").ToLower().Contains(lowerSearchTerm)
                ).ToList();
            }

            if (makeId.HasValue)
            {
                allCars = allCars.Where(c => c.Generation?.Model?.Make?.MakeId == makeId.Value).ToList();
            }

            if (modelId.HasValue)
            {
                allCars = allCars.Where(c => c.Generation?.Model?.ModelId == modelId.Value).ToList();
            }

            if (minPrice.HasValue)
            {
                allCars = allCars.Where(c => c.Price >= minPrice.Value).ToList();
            }

            if (maxPrice.HasValue)
            {
                allCars = allCars.Where(c => c.Price <= maxPrice.Value).ToList();
            }

            if (minYear.HasValue)
            {
                allCars = allCars.Where(c => c.ProductionYear >= minYear.Value).ToList();
            }

            if (maxYear.HasValue)
            {
                allCars = allCars.Where(c => c.ProductionYear <= maxYear.Value).ToList();
            }

            if (fuelTypeId.HasValue)
            {
                allCars = allCars.Where(c => c.FuelType.FuelTypeId == fuelTypeId.Value).ToList();
            }

            if (transmissionId.HasValue)
            {
                allCars = allCars.Where(c => c.TransmissionType.TransmissionTypeId == transmissionId.Value).ToList();
            }

            if (bodyTypeId.HasValue)
            {
                allCars = allCars.Where(c => c.BodyType != null && c.BodyType.BodyTypeId == bodyTypeId.Value).ToList();
            }

            if (colorId.HasValue)
            {
                allCars = allCars.Where(c => c.Color != null && c.Color.ColorId == colorId.Value).ToList();
            }

            if (drivetrainId.HasValue)
            {
                allCars = allCars.Where(c => c.Drivetrain != null && c.Drivetrain.DrivetrainId == drivetrainId.Value).ToList();
            }

            if (euroClassId.HasValue)
            {
                allCars = allCars.Where(c => c.EuroClass != null && c.EuroClass.EuroClassId == euroClassId.Value).ToList();
            }

            if (statusId.HasValue)
            {
                allCars = allCars.Where(c => c.CarStatus.CarStatusId == statusId.Value).ToList();
            }

            var totalCount = allCars.Count;
            var paginatedCars = allCars
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Build list of available models based on selected make
            var models = new List<dynamic>();
            if (makeId.HasValue)
            {
                models = _context.Models
                    .Where(m => m.Make.MakeId == makeId.Value)
                    .Cast<dynamic>()
                    .ToList();
            }
            else
            {
                models = _context.Models.Cast<dynamic>().ToList();
            }

            var paginatedList = new PaginatedList<CarModel>(paginatedCars, totalCount, pageIndex, pageSize)
            {
                SearchTerm = searchTerm,
                SelectedMakeId = makeId,
                SelectedModelId = modelId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinYear = minYear,
                MaxYear = maxYear,
                SelectedFuelTypeId = fuelTypeId,
                SelectedTransmissionId = transmissionId,
                SelectedBodyTypeId = bodyTypeId,
                SelectedColorId = colorId,
                SelectedDrivetrainId = drivetrainId,
                SelectedEuroClassId = euroClassId,
                SelectedStatusId = statusId,
                
                // Filter options from database
                Makes = _context.Makes.Cast<dynamic>().ToList(),
                Models = models,
                FuelTypes = _context.FuelTypes.Cast<dynamic>().ToList(),
                TransmissionTypes = _context.TransmissionTypes.Cast<dynamic>().ToList(),
                BodyTypes = _context.BodyTypes.Cast<dynamic>().ToList(),
                Colors = _context.Colors.Cast<dynamic>().ToList(),
                Drivetrains = _context.Drivetrains.Cast<dynamic>().ToList(),
                EuroClasses = _context.EuroClasses.Cast<dynamic>().ToList(),
                CarStatuses = _context.CarStatus.Cast<dynamic>().ToList()
            };

            return View(paginatedList);
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
