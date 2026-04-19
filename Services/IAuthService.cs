namespace CarDealershipManager.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateAdminCredentialsAsync(string userName, string password);
        Task<Models.Entities.AdminModel?> GetAdminByUserNameAsync(string userName);
        Task<bool> UpdateAdminSettingsAsync(string userName, string currentPassword, string? newEmail, string? newPassword);
        Task<bool> ResetPasswordByEmailAsync(string email);
    }
}
