using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrainOperationAPI.Models
{
    public class FarmerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("farmer_id")]
        public int FarmerId { get; set; } // Primary key
        [Column("FarmerFirstName")]
        public string? FarmerFirstName { get; set; }
        [Column("FarmerLastName")]
        public string? FarmerLastName { get; set; }

        // Navigation property
        public ICollection<TruckModel>? Trucks { get; set; } // One-to-many relationship with trucks
    }
}
