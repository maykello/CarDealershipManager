namespace CarDealershipManager.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateAdminCredentialsAsync(string userName, string password);
    }
}
