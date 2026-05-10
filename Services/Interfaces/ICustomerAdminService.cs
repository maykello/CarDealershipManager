using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Services.Interfaces
{
    public interface ICustomerAdminService
    {
        Task<IEnumerable<CustomerModel>> GetAllCustomersAsync();
        Task<CustomerModel?> GetCustomerByIdAsync(int id);
        Task<CustomerModel> CreateCustomerAsync(CustomerModel customer);
        Task UpdateCustomerAsync(CustomerModel customer);
        Task DeleteCustomerAsync(int id);
        Task<bool> CustomerExistsAsync(int id);
    }
}
