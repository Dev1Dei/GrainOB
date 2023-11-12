using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using GrainOperationAPI.Models.DTOs;

namespace GrainOperationAPI.Services
{
    public class ConversionService
    {
            private readonly GrainOperationContext _context;

            public ConversionService(GrainOperationContext context)
            {
                _context = context;
            }

            public TransactionModel ConvertToTransactionModel(TransactionDto dto)
            {
                // Retrieve or create Farmer
                var farmer = _context.Farmers
                    .FirstOrDefault(f => f.FarmerFirstName == dto.FarmerFirstName && f.FarmerLastName == dto.FarmerLastName)
                    ?? new FarmerModel { FarmerFirstName = dto.FarmerFirstName, FarmerLastName = dto.FarmerLastName };

                if (farmer.FarmerId == 0)
                {
                    _context.Farmers.Add(farmer);
                    // Assuming we don't immediately save changes to allow for a transactional approach
                }

                // Retrieve or create Truck
                var truck = _context.Trucks
                    .FirstOrDefault(t => t.TruckNumbers == dto.TruckNumbers)
                    ?? new TruckModel { TruckNumbers = dto.TruckNumbers, TruckStorage = dto.TruckStorage, Farmer = farmer };

                if (truck.TruckId == 0)
                {
                    // If we created a new truck, add it to the context
                    _context.Trucks.Add(truck);
                }

                // Assuming that the transaction is always new
                var transaction = new TransactionModel
                {
                    Truck = truck,
                    GrainType = dto.Grain.Type,
                    GrainClass = dto.Grain.Class,
                    Dryness = dto.Grain.Dryness,
                    Cleanliness = dto.Grain.Cleanliness,
                    GrainWeight = dto.GrainWeight,
                    ArrivalTime = DateTime.Parse(dto.Arrival), // You should handle parse errors in production code
                    WantedPay = dto.WantedPay,
                    PricePerTonne = dto.PricePerTonne,
                    Status = "Pending" // Assuming a default status of "Pending"
                };

                return transaction;
            }

            public void ApplyChanges()
            {
                _context.SaveChanges();
            }
    }
}
