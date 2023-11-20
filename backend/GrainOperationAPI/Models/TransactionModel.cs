using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrainOperationAPI.Models
{
    public class TransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("transaction_id")]
        public int TransactionId { get; set; } // Primary Key
        [ForeignKey(nameof(Truck))]
        [Column("truck_id")]
        public int TruckId { get; set; } // Foreign Key
        [Column("grain_type")]
        public string GrainType { get; set; }
        [Column("grain_class")]
        public string GrainClass { get; set; } // This can be null for certain types of grains
        [Column("Dryness")]
        public double Dryness { get; set; }
        [Column("Cleanliness")]
        public double Cleanliness { get; set; }
        [Column("grain_weight")]
        public double GrainWeight { get; set; }
        [Column("Arrival")]
        public DateTime ArrivalTime { get; set; }
        [Column("WantedPay")]
        public double WantedPay { get; set; }
        [Column("price_per_tonne")]
        public double PricePerTonne { get; set; }
        [Column("status")]
        public string? Status { get; set; } = "Pending"; // This can be 'Accepted', 'Denied', or null

        // Navigation properties
        public TruckModel Truck { get; set; } // Navigation property back to TruckModel
    }
}
