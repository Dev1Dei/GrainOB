using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrainOperationAPI.Models
{
    public class PriceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceId { get; set; }

        [Required]
        public string GrainType { get; set; }

        public string GrainClass { get; set; } // This can be nullable

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }

}
