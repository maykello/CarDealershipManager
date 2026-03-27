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

        public IActionResult Cars()
        {
            var cars = _context.Cars
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

            return View(cars);
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
