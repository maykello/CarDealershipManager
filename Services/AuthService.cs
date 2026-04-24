using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
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
    }
}
