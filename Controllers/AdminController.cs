using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarDealershipManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
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

        [HttpPost]
        [Authorize]
        public IActionResult KeepAlive()
        {
            return Ok();
        }
    }
}
