using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using GrainOperationAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GrainOperationAPI.Services
{
    public class ConversionService
    {
        private readonly GrainOperationContext _context;

        public ConversionService(GrainOperationContext context)
        {
            _context = context;
        }

        public async Task<TransactionModel> ConvertToTransactionModel(TransactionDto dto)
        {
            // Retrieve or create Farmer
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.FarmerFirstName == dto.FarmerFirstName && f.FarmerLastName == dto.FarmerLastName)
                ?? new FarmerModel { FarmerFirstName = dto.FarmerFirstName, FarmerLastName = dto.FarmerLastName };

            // Retrieve or create Truck
            var truck = await _context.Trucks
                .FirstOrDefaultAsync(t => t.TruckNumbers == dto.TruckNumbers)
                ?? new TruckModel { TruckNumbers = dto.TruckNumbers, TruckStorage = dto.TruckStorage, Farmer = farmer };

            // Create the transaction
            var transaction = new TransactionModel
            {
                Truck = truck,
                GrainType = dto.Grain.Type,
                GrainClass = dto.Grain.Class,
                Dryness = dto.Grain.Dryness,
                Cleanliness = dto.Grain.Cleanliness,
                GrainWeight = dto.GrainWeight,
                ArrivalTime = DateTime.Parse(dto.Arrival),
                WantedPay = dto.WantedPay,
                PricePerTonne = dto.PricePerTonne,
                Status = "Pending"
            };

            _context.Transactions.Add(transaction);

            // Save all changes to the context
            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
