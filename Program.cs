using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Pobranie stringa z appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Rejestracja DbContext
builder.Services.AddDbContext<CarDealershipDbContext>(options =>
    options.UseSqlServer(connectionString));

// Rejestracja AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});

builder.Services.Configure<CarDealershipManager.Models.Settings.SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<CarDealershipManager.Models.Settings.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Rejestracja serwisów
builder.Services.AddScoped<ICarSearchService, CarSearchService>();
builder.Services.AddScoped<IFilterService, FilterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICarAdminService, CarAdminService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IContractService, ContractService>();

// Serwer przechowuje dane szyfrujące wyłącznie w RAM - odcięcie zasilania to automatyczne wylogowanie wszystkich
builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

// Konfiguracja uwierzytelniania ciasteczkami
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Seedowanie domyślnego konta admina, jeśli nie istnieje
await DataSeeder.SeedAdminAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
