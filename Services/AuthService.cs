using CarDealershipManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class AuthService : IAuthService
    {
        private readonly CarDealershipDbContext _context;

        public AuthService(CarDealershipDbContext context)
        {
            _context = context;
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
    }
}
