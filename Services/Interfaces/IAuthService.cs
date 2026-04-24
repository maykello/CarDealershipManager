namespace CarDealershipManager.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateAdminCredentialsAsync(string userName, string password);
    }
}
