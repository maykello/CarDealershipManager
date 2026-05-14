using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.ViewModels;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealershipManager.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly IAuthService _authService;

        public UserManagementController(IAuthService authService)
        {
            _authService = authService;
        }

        private bool IsMainAdmin() => User.Identity?.Name == "admin";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsMainAdmin()) return Forbid();

            var admins = await _authService.GetAllAdminsAsync();
            return View(admins);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsMainAdmin()) return Forbid();

            return View(new CreateUserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            if (!IsMainAdmin()) return Forbid();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sprawdź czy login nie jest już zajęty
            var existingAdmins = await _authService.GetAllAdminsAsync();
            if (existingAdmins.Any(a => a.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("UserName", "Ten login jest już zajęty.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && existingAdmins.Any(a => a.Email != null && a.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Email", "Ten adres email jest już używany.");
                return View(model);
            }

            var (user, generatedPassword) = await _authService.CreateUserAsync(model.UserName, model.Email);

            var viewModel = new UserCreatedViewModel
            {
                UserName = user.UserName,
                GeneratedPassword = generatedPassword
            };

            return View("UserCreated", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsMainAdmin()) return Forbid();

            // Nie pozwól usunąć konta "admin"
            var targetAdmin = await _authService.GetAdminByUserNameAsync("admin");
            if (targetAdmin != null && targetAdmin.UserId == id)
            {
                TempData["ErrorMessage"] = "Nie można usunąć głównego konta administratora.";
                return RedirectToAction("Index");
            }

            var success = await _authService.DeleteAdminAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Użytkownik został usunięty.";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie udało się usunąć użytkownika.";
            }

            return RedirectToAction("Index");
        }
    }
}
