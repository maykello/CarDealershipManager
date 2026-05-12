using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class AuthService : IAuthService
    {
        private readonly CarDealershipDbContext _context;
        private readonly IEmailService _emailService;

        public AuthService(CarDealershipDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> ValidateAdminCredentialsAsync(string userName, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserName == userName);

            if (admin == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, admin.Password);
        }

        public async Task<AdminModel?> GetAdminByUserNameAsync(string userName)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.UserName == userName);
        }

        public async Task<bool> UpdateAdminSettingsAsync(string userName, string currentPassword, string? newEmail, string? newPassword)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserName == userName);
            if (admin == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, admin.Password))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                admin.Email = newEmail;
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordByEmailAsync(string email)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null)
            {
                return false;
            }

            var passwordChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            var newPassword = new string(Enumerable.Repeat(passwordChars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            var subject = "Reset hasła CarDealershipManager";
            var body = $"<p>Witaj {admin.UserName},</p><p>Oto Twoje nowe, tymczasowe hasło: <strong>{newPassword}</strong></p><p>Zalecamy jego zmianę po zalogowaniu.</p>";

            await _emailService.SendEmailAsync(admin.Email, subject, body);

            return true;
        }
    }
}
