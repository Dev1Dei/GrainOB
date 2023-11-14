namespace GrainOperationAPI.Models.DTOs
{
    public class CreationEventDTO
    {
        public string FarmerFirstName { get; set; }
        public string FarmerLastName { get; set; }
        public string TruckNumbers { get; set; }
        public int TruckStorage { get; set; }
        public string? GrainType { get; set; }
        public string? GrainClass { get; set; }
        public double Dryness { get; set; }
        public double Cleanliness { get; set; }
        public double GrainWeight { get; set; }
        public string? ArrivalTime { get; set; }
        public double WantedPay { get; set; }
        public double PricePerTonne { get; set; }
        // Add any additional properties that the client might need
    }
}
