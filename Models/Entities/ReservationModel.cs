using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipManager.Models.Entities
{
    public class ReservationModel
    {
        [Key]
        public int Id { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required DateTime ExpirationDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deposit { get; set; }
        public required CarModel Car { get; set; }
        public required CustomerModel Customer { get; set; }
    }
}
