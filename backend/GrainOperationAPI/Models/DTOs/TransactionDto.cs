namespace GrainOperationAPI.Models.DTOs
{
    public class TransactionDto
    {
        public string FarmerFirstName { get; set; }
        public string FarmerLastName { get; set; }
        public string TruckNumbers { get; set; }
        public int TruckStorage { get; set; }
        public GrainDto Grain { get; set; }
        public decimal GrainWeight { get; set; }
        public string Arrival { get; set; }
        public double WantedPay { get; set; }
        public double PricePerTonne { get; set; }
    }
}
