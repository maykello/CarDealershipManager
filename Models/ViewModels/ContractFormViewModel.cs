using Microsoft.AspNetCore.Mvc.Rendering;
using CarDealershipManager.Models.Dtos;
using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Models.ViewModels
{
    public class ContractFormViewModel
    {
        public ContractDto Contract { get; set; } = new ContractDto();
        public IEnumerable<SelectListItem> Cars { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TransactionTypes { get; set; } = new List<SelectListItem>();
    }
}
