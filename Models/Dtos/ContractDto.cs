using CarDealershipManager.Models.Entities;

namespace CarDealershipManager.Models.Dtos
{
    public class ContractDto
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Price { get; set; }
        public TransactionType TransactionType { get; set; }

        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public int AdminId { get; set; }

        public CarDto? Car { get; set; }
        public CustomerDto? Customer { get; set; }
        public AdminDto? Admin { get; set; }
    }
}
