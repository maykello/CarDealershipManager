using Microsoft.EntityFrameworkCore;
using CarDealershipManager.Models.Entities;
using Xunit;

public class DatabaseTests
{
    [Fact]
    public async Task Database_CanConnect_ReturnsTrue()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        var options = new DbContextOptionsBuilder<CarDealershipDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var context = new CarDealershipDbContext(options);
        bool canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect, "Nie można połączyć się z zewnętrzną bazą!");
    }
}