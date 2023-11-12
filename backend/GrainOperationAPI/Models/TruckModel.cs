using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrainOperationAPI.Models
{
    public class TruckModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("truck_id")]
        public int TruckId { get; set; } // Primary Key
        [ForeignKey("Farmer")]
        [Column("farmer_id")]
        public int FarmerId { get; set; } // Foreign Key
        [Column("TruckNumbers")]
        public string TruckNumbers { get; set; }
        [Column("TruckStorage")]
        public int TruckStorage { get; set; }

        // Navigation properties
        public FarmerModel Farmer { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; } // One-to-Many relationship
    }
}
