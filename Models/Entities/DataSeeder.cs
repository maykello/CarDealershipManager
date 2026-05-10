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

        public static async Task SeedCompanyPlaceholderAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CarDealershipDbContext>();

            if (!await context.Customers.AnyAsync())
            {
                var companyPlaceholder = new CustomerModel
                {
                    Type = CustomerType.Company,
                    CompanyName = "Nazwa Naszej Firmy (Placeholder)",
                    TaxId = "1234567890",
                    Country = "Polska",
                    City = "Warszawa",
                    AddressLine1 = "ul. Przykładowa 1",
                    PostalCode = "00-001",
                    Email = "kontakt@naszafirma.pl",
                    PhoneNumber = "+48 123 456 789"
                };

                context.Customers.Add(companyPlaceholder);
                await context.SaveChangesAsync();
            }
        }
    }
}
