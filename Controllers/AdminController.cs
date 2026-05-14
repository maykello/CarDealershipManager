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
        private readonly IContractService _contractService;
        private readonly ICustomerAdminService _customerAdminService;

        public AdminController(IAuthService authService, ICarAdminService carAdminService, IContractService contractService, ICustomerAdminService customerAdminService)
        {
            _authService = authService;
            _carAdminService = carAdminService;
            _contractService = contractService;
            _customerAdminService = customerAdminService;
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
                var admin = await _authService.ValidateAndGetAdminAsync(model.UserName, model.Password);

                if (admin != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.UserName),
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

        [HttpPost]
        [Authorize]
        public IActionResult KeepAlive()
        {
            return Ok();
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenerateInvoice(int carId)
        {
            var car = await _carAdminService.GetCarByIdAsync(carId);
            if (car == null) return NotFound();

            var customers = await _customerAdminService.GetAllCustomersAsync();
            var customerSelectList = customers
                .Where(c => c.Id != 1) // Omijamy naszą firmę
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Type == CarDealershipManager.Models.Entities.CustomerType.Company 
                        ? $"{c.CompanyName} (NIP: {c.TaxId})" 
                        : $"{c.FirstName} {c.LastName} (PESEL: {c.NationalIdNumber})"
                });

            var viewModel = new ContractGenerationViewModel
            {
                CarId = carId,
                Car = car,
                Price = car.Price,
                ExistingCustomers = customerSelectList
            };

            return View("GenerateInvoice", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoice(ContractGenerationViewModel model)
        {
            var car = await _carAdminService.GetCarByIdAsync(model.CarId);
            if (car == null) return NotFound();

            CarDealershipManager.Models.Entities.CustomerModel buyer;

            if (model.IsNewCustomer && model.NewCustomer != null)
            {
                buyer = await _customerAdminService.CreateCustomerAsync(model.NewCustomer);
            }
            else if (model.SelectedCustomerId.HasValue)
            {
                buyer = await _customerAdminService.GetCustomerByIdAsync(model.SelectedCustomerId.Value);
                if (buyer == null) return BadRequest("Nie znaleziono wybranego klienta.");
            }
            else
            {
                ModelState.AddModelError("", "Wybierz klienta lub dodaj nowego.");
                var customersList = await _customerAdminService.GetAllCustomersAsync();
                model.ExistingCustomers = customersList.Where(c => c.Id != 1).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Type == CarDealershipManager.Models.Entities.CustomerType.Company ? c.CompanyName : $"{c.FirstName} {c.LastName}"
                });
                model.Car = car;
                return View("GenerateInvoice", model);
            }

            var seller = await _customerAdminService.GetCustomerByIdAsync(1); // Nasza firma
            if (seller == null) return BadRequest("Nie skonfigurowano profilu firmy (klient ID=1).");

            var userName = User.Identity?.Name;
            var currentAdmin = await _authService.GetAdminByUserNameAsync(userName ?? "");
            if (currentAdmin == null) return BadRequest("Nie odnaleziono zalogowanego administratora.");

            var invoiceNumber = await _contractService.GenerateInvoiceNumberAsync();
            var pdfBytes = await _contractService.GenerateInvoicePdfAsync(invoiceNumber, car, buyer, seller, model.Price, model.PaymentMethod, model.BankAccountNumber);
            
            // Zapisz do bazy
            var contractDto = new ContractDto
            {
                ContractNumber = invoiceNumber,
                TransactionDate = DateTime.Now,
                Price = model.Price,
                TransactionType = CarDealershipManager.Models.Entities.TransactionType.Sell,
                CarId = car.CarId,
                CustomerId = buyer.Id,
                AdminId = currentAdmin.UserId
            };
            
            await _contractService.CreateContractAsync(contractDto);

            var safeNumber = invoiceNumber.Replace("/", "-");
            var fileName = $"Faktura_{safeNumber}_{DateTime.Now:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
