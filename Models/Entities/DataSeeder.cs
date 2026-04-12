using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Models.Entities
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CarDealershipDbContext>();

            if (!await context.Admins.AnyAsync())
            {
                var defaultAdmin = new AdminModel
                {
                    UserName = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                };

                context.Admins.Add(defaultAdmin);
                await context.SaveChangesAsync();
            }
        }
    }
}
