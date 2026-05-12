using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipManager.Models.Entities
{
    public enum TransactionType
    {
        Buy,
        Sell
    }
    public class ContractModel
    {
        [Key]
        public int Id { get; set; }
        public required string ContractNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }
        public required TransactionType Type { get; set; }
        public required CarModel Car { get; set; }
        public required CustomerModel Customer { get; set; }
        public required AdminModel Admin { get; set; }

    }
}
