using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CarDealershipManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ICarAdminService _carAdminService;

        public AdminController(IAuthService authService, ICarAdminService carAdminService)
        {
            _authService = authService;
            _carAdminService = carAdminService;
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
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return View(carDto);
            }

            await _carAdminService.CreateCarAsync(carDto);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var carDto = await _carAdminService.GetCarByIdAsync(id);

            if (carDto == null)
            {
                return NotFound();
            }

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

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return View(carDto);
            }

            try
            {
                var exists = await _carAdminService.CarExistsAsync(id);
                if (!exists)
                {
                    return NotFound();
                }

                await _carAdminService.UpdateCarAsync(id, carDto);
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _carAdminService.DeleteCarAsync(id);
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetModelsByMake(int makeId)
        {
            var models = await _carAdminService.GetModelsByMakeAsync(makeId);
            return Json(models);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGenerationsByModel(int modelId)
        {
            var generations = await _carAdminService.GetGenerationsByModelAsync(modelId);
            return Json(generations);
        }

        private async Task PopulateSelectListsAsync()
        {
            ViewBag.MakeId = new SelectList(await _carAdminService.GetMakesAsync(), "Id", "Name");
            ViewBag.ModelId = new SelectList(await _carAdminService.GetModelsAsync(), "Id", "Name");
            ViewBag.GenerationId = new SelectList(await _carAdminService.GetGenerationsAsync(), "Id", "Name");
            ViewBag.TransmissionTypeId = new SelectList(await _carAdminService.GetTransmissionTypesAsync(), "Id", "Name");
            ViewBag.DrivetrainId = new SelectList(await _carAdminService.GetDrivetrainsAsync(), "Id", "Name");
            ViewBag.BodyTypeId = new SelectList(await _carAdminService.GetBodyTypesAsync(), "Id", "Name");
            ViewBag.EuroClassId = new SelectList(await _carAdminService.GetEuroClassesAsync(), "Id", "Name");
            ViewBag.ColorId = new SelectList(await _carAdminService.GetColorsAsync(), "Id", "Name");
            ViewBag.FuelTypeId = new SelectList(await _carAdminService.GetFuelTypesAsync(), "Id", "Name");
            ViewBag.CarStatusId = new SelectList(await _carAdminService.GetCarStatusesAsync(), "Id", "Name");
        }
    }
}
