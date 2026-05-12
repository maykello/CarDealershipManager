using CarDealershipManager.Models.Entities;
using CarDealershipManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealershipManager.Controllers
{
    [Authorize]
    public class CustomerAdminController : Controller
    {
        private readonly ICustomerAdminService _customerAdminService;

        public CustomerAdminController(ICustomerAdminService customerAdminService)
        {
            _customerAdminService = customerAdminService;
        }

        // GET: CustomerAdmin
        public async Task<IActionResult> Index()
        {
            var customers = await _customerAdminService.GetAllCustomersAsync();
            return View(customers);
        }

        // GET: CustomerAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,FirstName,LastName,CompanyName,NationalIdNumber,TaxId,DocumentNumber,PhoneNumber,Email,Country,City,PostalCode,AddressLine1,AddressLine2")] CustomerModel customerModel)
        {
            if (ModelState.IsValid)
            {
                await _customerAdminService.CreateCustomerAsync(customerModel);
                return RedirectToAction(nameof(Index));
            }
            return View(customerModel);
        }

        // GET: CustomerAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _customerAdminService.GetCustomerByIdAsync(id.Value);
            if (customerModel == null)
            {
                return NotFound();
            }
            return View(customerModel);
        }

        // POST: CustomerAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,FirstName,LastName,CompanyName,NationalIdNumber,TaxId,DocumentNumber,PhoneNumber,Email,Country,City,PostalCode,AddressLine1,AddressLine2")] CustomerModel customerModel)
        {
            if (id != customerModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _customerAdminService.UpdateCustomerAsync(customerModel);
                }
                catch (KeyNotFoundException)
                {
                    if (!await _customerAdminService.CustomerExistsAsync(customerModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customerModel);
        }

        // GET: CustomerAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _customerAdminService.GetCustomerByIdAsync(id.Value);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: CustomerAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ochrona przed usunięciem rekordu o ID=1 (nasza firma)
            if (id == 1)
            {
                TempData["ErrorMessage"] = "Nie można usunąć rekordu reprezentującego naszą firmę (ID=1).";
                return RedirectToAction(nameof(Index));
            }

            await _customerAdminService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
