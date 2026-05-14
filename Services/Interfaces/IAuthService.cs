namespace CarDealershipManager.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateAdminCredentialsAsync(string userName, string password);
        Task<Models.Entities.AdminModel?> GetAdminByUserNameAsync(string userName);
        Task<Models.Entities.AdminModel?> ValidateAndGetAdminAsync(string usernameOrEmail, string password);
        Task<bool> UpdateAdminSettingsAsync(string userName, string currentPassword, string? newEmail, string? newPassword);
        Task<bool> ResetPasswordByEmailAsync(string email);
        Task<(Models.Entities.AdminModel user, string generatedPassword)> CreateUserAsync(string userName, string? email);
        Task<List<Models.Entities.AdminModel>> GetAllAdminsAsync();
        Task<bool> DeleteAdminAsync(int userId);
    }
}
