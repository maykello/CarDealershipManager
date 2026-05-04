using CarDealershipManager.Models;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.ViewModels;
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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var success = await _authService.ResetPasswordByEmailAsync(model.Email);
                if (success)
                {
                    ViewBag.SuccessMessage = "Jeśli podany email istnieje w bazie, wysłano na niego nowe, tymczasowe hasło.";
                    ModelState.Clear();
                    return View(new ForgotPasswordDto());
                }
                else
                {
                    ViewBag.SuccessMessage = "Jeśli podany email istnieje w bazie, wysłano na niego nowe, tymczasowe hasło.";
                    ModelState.Clear();
                    return View(new ForgotPasswordDto());
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Panel()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login");

            var admin = await _authService.GetAdminByUserNameAsync(userName);
            if (admin == null) return RedirectToAction("Login");

            var model = new AdminSettingsDto
            {
                Email = admin.Email
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Panel(AdminSettingsDto model)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("ConfirmNewPassword", "Pola nowych haseł nie są identyczne.");
                return View(model);
            }

            var success = await _authService.UpdateAdminSettingsAsync(userName, model.CurrentPassword, model.Email, model.NewPassword);

            if (success)
            {
                ViewBag.SuccessMessage = "Ustawienia zostały pomyślnie zapisane.";
                ModelState.Clear();
                model.CurrentPassword = "";
                model.NewPassword = "";
                model.ConfirmNewPassword = "";
                return View(model);
            }
            else
            {
                ModelState.AddModelError("CurrentPassword", "Obecne hasło jest nieprawidłowe.");
                return View(model);
            }
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
            var viewModel = new CarFormViewModel();
            await PopulateSelectListsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(viewModel);
                return View(viewModel);
            }

            await _carAdminService.CreateCarAsync(viewModel.Car, viewModel.Photos);
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

            var viewModel = new CarFormViewModel { Car = carDto };
            await PopulateSelectListsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarFormViewModel viewModel)
        {
            if (id != viewModel.Car.CarId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(viewModel);
                return View(viewModel);
            }

            try
            {
                var exists = await _carAdminService.CarExistsAsync(id);
                if (!exists)
                {
                    return NotFound();
                }

                await _carAdminService.UpdateCarAsync(id, viewModel.Car, viewModel.Photos, viewModel.MainPhotoFilename);
                return RedirectToAction("Details", "Home", new { id = id });
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var success = await _carAdminService.DeletePhotoByIdAsync(photoId);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainPhoto(int carId, int photoId)
        {
            var success = await _carAdminService.SetMainPhotoAsync(carId, photoId);
            if (success) return Ok();
            return BadRequest();
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

        private async Task PopulateSelectListsAsync(CarFormViewModel viewModel)
        {
            viewModel.Makes = new SelectList(await _carAdminService.GetMakesAsync(), "Id", "Name");
            viewModel.Models = new SelectList(await _carAdminService.GetModelsAsync(), "Id", "Name");
            viewModel.Generations = new SelectList(await _carAdminService.GetGenerationsAsync(), "Id", "Name");
            viewModel.TransmissionTypes = new SelectList(await _carAdminService.GetTransmissionTypesAsync(), "Id", "Name");
            viewModel.Drivetrains = new SelectList(await _carAdminService.GetDrivetrainsAsync(), "Id", "Name");
            viewModel.BodyTypes = new SelectList(await _carAdminService.GetBodyTypesAsync(), "Id", "Name");
            viewModel.EuroClasses = new SelectList(await _carAdminService.GetEuroClassesAsync(), "Id", "Name");
            viewModel.Colors = new SelectList(await _carAdminService.GetColorsAsync(), "Id", "Name");
            viewModel.FuelTypes = new SelectList(await _carAdminService.GetFuelTypesAsync(), "Id", "Name");
            viewModel.CarStatuses = new SelectList(await _carAdminService.GetCarStatusesAsync(), "Id", "Name");
        }
    }
}
