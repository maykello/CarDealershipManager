using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Services
{
    public class CustomerAdminService : ICustomerAdminService
    {
        private readonly CarDealershipDbContext _context;

        public CustomerAdminService(CarDealershipDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerModel>> GetAllCustomersAsync()
        {
            return await _context.Customers.OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<CustomerModel?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CustomerModel> CreateCustomerAsync(CustomerModel customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateCustomerAsync(CustomerModel customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customer.Id} not found.");
            }

            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }
        }

        public async Task<bool> CustomerExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
