using AutoMapper;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarDealershipManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly CarDealershipDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(IAuthService authService, CarDealershipDbContext context, IMapper mapper)
        {
            _authService = authService;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                bool isValid = await _authService.ValidateAdminCredentialsAsync(model.UserName, model.Password);

                if (isValid)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Nieprawidłowy login lub hasło.");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // CRUD Operations for Cars

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            const int pageSize = 10;
            var carsQuery = _context.Cars
                .Include(c => c.Generation)
                    .ThenInclude(g => g.Model)
                        .ThenInclude(m => m.Make)
                .Include(c => c.FuelType)
                .Include(c => c.CarStatus)
                .OrderByDescending(c => c.CarId);

            var cars = await carsQuery.ToListAsync();
            var carDtos = _mapper.Map<List<CarDto>>(cars);

            // Paginacja
            var pagedCars = carDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling(carDtos.Count / (double)pageSize);

            return View(pagedCars);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View(new CarDto());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarDto carDto)
        {
            // Debug - sprawdzenie błędów walidacji
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return View(carDto);
            }

            var car = _mapper.Map<CarModel>(carDto);

            // Set navigation properties from DTO flat ID properties
            if (carDto.GenerationId.HasValue && carDto.GenerationId > 0)
                car.Generation = await _context.Generations.FindAsync(carDto.GenerationId.Value);

            if (carDto.FuelTypeId.HasValue && carDto.FuelTypeId > 0)
                car.FuelType = await _context.FuelTypes.FindAsync(carDto.FuelTypeId.Value);

            if (carDto.CarStatusId.HasValue && carDto.CarStatusId > 0)
                car.CarStatus = await _context.CarStatus.FindAsync(carDto.CarStatusId.Value);

            if (carDto.TransmissionTypeId.HasValue && carDto.TransmissionTypeId > 0)
                car.TransmissionType = await _context.TransmissionTypes.FindAsync(carDto.TransmissionTypeId.Value);

            if (carDto.DrivetrainId.HasValue && carDto.DrivetrainId > 0)
                car.Drivetrain = await _context.Drivetrains.FindAsync(carDto.DrivetrainId.Value);

            if (carDto.BodyTypeId.HasValue && carDto.BodyTypeId > 0)
                car.BodyType = await _context.BodyTypes.FindAsync(carDto.BodyTypeId.Value);

            if (carDto.ColorId.HasValue && carDto.ColorId > 0)
                car.Color = await _context.Colors.FindAsync(carDto.ColorId.Value);

            if (carDto.EuroClassId.HasValue && carDto.EuroClassId > 0)
                car.EuroClass = await _context.EuroClasses.FindAsync(carDto.EuroClassId.Value);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Generation)
                    .ThenInclude(g => g.Model)
                        .ThenInclude(m => m.Make)
                .Include(c => c.FuelType)
                .Include(c => c.TransmissionType)
                .Include(c => c.Drivetrain)
                .Include(c => c.BodyType)
                .Include(c => c.Color)
                .Include(c => c.EuroClass)
                .Include(c => c.CarStatus)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            var carDto = _mapper.Map<CarDto>(car);
            await PopulateSelectListsAsync();
            return View(carDto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarDto carDto)
        {
            if (id != carDto.CarId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var car = await _context.Cars.FindAsync(id);
                    if (car == null)
                    {
                        return NotFound();
                    }

                    _mapper.Map(carDto, car);

                    // Set navigation properties from DTO flat ID properties
                    if (carDto.GenerationId.HasValue && carDto.GenerationId > 0)
                        car.Generation = await _context.Generations.FindAsync(carDto.GenerationId.Value);
                    else
                        car.Generation = null;

                    if (carDto.FuelTypeId.HasValue && carDto.FuelTypeId > 0)
                        car.FuelType = await _context.FuelTypes.FindAsync(carDto.FuelTypeId.Value);
                    else
                        car.FuelType = null;

                    if (carDto.CarStatusId.HasValue && carDto.CarStatusId > 0)
                        car.CarStatus = await _context.CarStatus.FindAsync(carDto.CarStatusId.Value);
                    else
                        car.CarStatus = null;

                    if (carDto.TransmissionTypeId.HasValue && carDto.TransmissionTypeId > 0)
                        car.TransmissionType = await _context.TransmissionTypes.FindAsync(carDto.TransmissionTypeId.Value);
                    else
                        car.TransmissionType = null;

                    if (carDto.DrivetrainId.HasValue && carDto.DrivetrainId > 0)
                        car.Drivetrain = await _context.Drivetrains.FindAsync(carDto.DrivetrainId.Value);
                    else
                        car.Drivetrain = null;

                    if (carDto.BodyTypeId.HasValue && carDto.BodyTypeId > 0)
                        car.BodyType = await _context.BodyTypes.FindAsync(carDto.BodyTypeId.Value);
                    else
                        car.BodyType = null;

                    if (carDto.ColorId.HasValue && carDto.ColorId > 0)
                        car.Color = await _context.Colors.FindAsync(carDto.ColorId.Value);
                    else
                        car.Color = null;

                    if (carDto.EuroClassId.HasValue && carDto.EuroClassId > 0)
                        car.EuroClass = await _context.EuroClasses.FindAsync(carDto.EuroClassId.Value);
                    else
                        car.EuroClass = null;

                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(carDto.CarId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            await PopulateSelectListsAsync();
            return View(carDto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetModelsByMake(int makeId)
        {
            var models = await _context.Models
                .Where(m => m.MakeId == makeId)
                .Select(m => new { id = m.ModelId, name = m.Name })
                .ToListAsync();
            return Json(models);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGenerationsByModel(int modelId)
        {
            var generations = await _context.Generations
                .Where(g => g.ModelId == modelId)
                .Select(g => new { id = g.GenerationId, name = g.Name })
                .ToListAsync();
            return Json(generations);
        }

        private async Task PopulateSelectListsAsync()
        {
            // Marki (Make)
            ViewBag.MakeId = new SelectList(
                await _context.Makes.ToListAsync(),
                "MakeId",
                "Name"
            );

            // Modele (Model)
            ViewBag.ModelId = new SelectList(
                await _context.Models.ToListAsync(),
                "ModelId",
                "Name"
            );

            // Generacje (Generation)
            ViewBag.GenerationId = new SelectList(
                await _context.Generations.ToListAsync(),
                "GenerationId",
                "Name"
            );

            ViewBag.TransmissionTypeId = new SelectList(
                await _context.TransmissionTypes.ToListAsync(),
                "TransmissionTypeId",
                "Name"
            );

            ViewBag.DrivetrainId = new SelectList(
                await _context.Drivetrains.ToListAsync(),
                "DrivetrainId",
                "Name"
            );

            ViewBag.BodyTypeId = new SelectList(
                await _context.BodyTypes.ToListAsync(),
                "BodyTypeId",
                "Name"
            );

            ViewBag.EuroClassId = new SelectList(
                await _context.EuroClasses.ToListAsync(),
                "EuroClassId",
                "Name"
            );

            ViewBag.ColorId = new SelectList(
                await _context.Colors.ToListAsync(),
                "ColorId",
                "Name"
            );

            ViewBag.FuelTypeId = new SelectList(
                await _context.FuelTypes.ToListAsync(),
                "FuelTypeId",
                "Name"
            );

            ViewBag.CarStatusId = new SelectList(
                await _context.CarStatus.ToListAsync(),
                "CarStatusId",
                "Name"
            );
        }
    }
}
