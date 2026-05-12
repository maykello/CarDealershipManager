using System;
using System.Threading.Tasks;
using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarDealershipManager.UnitTests
{
    public class AuthServiceTests
    {
        private CarDealershipDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CarDealershipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new CarDealershipDbContext(options);
        }

        [Fact]
        public async Task ValidateAdminCredentialsAsync_DlaPoprawnychDanych_ZwracaTrue()
        {
            var dbContext = GetInMemoryDbContext();

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("MojePrawdziweHaslo123");
            
            dbContext.Admins.Add(new AdminModel 
            { 
                UserId = 1, 
                UserName = "superAdmin", 
                Password = hashedPassword 
            });
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, null!);

            var isCredentialValid = await authService.ValidateAdminCredentialsAsync("superAdmin", "MojePrawdziweHaslo123");

            Assert.True(isCredentialValid);
        }

        [Fact]
        public async Task ValidateAdminCredentialsAsync_DlaZlegoHasla_ZwracaFalse()
        {
            var dbContext = GetInMemoryDbContext();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("MojePrawdziweHaslo123");
            
            dbContext.Admins.Add(new AdminModel 
            { 
                UserId = 2, 
                UserName = "zwyklyAdmin", 
                Password = hashedPassword 
            });
            await dbContext.SaveChangesAsync();

            var authService = new AuthService(dbContext, null!);

            var isCredentialValid = await authService.ValidateAdminCredentialsAsync("zwyklyAdmin", "ZleHaslo1!");

            Assert.False(isCredentialValid);
        }

        [Fact]
        public async Task ValidateAdminCredentialsAsync_GdyNieMaTakiegoUzytkownika_ZwracaFalse()
        {
            var dbContext = GetInMemoryDbContext();
            
            var authService = new AuthService(dbContext, null!);

            var isCredentialValid = await authService.ValidateAdminCredentialsAsync("nieistnieje", "ZleHaslo1!");

            Assert.False(isCredentialValid);
        }
    }
}
