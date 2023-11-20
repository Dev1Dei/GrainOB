using GrainOperationAPI.Data;
using GrainOperationAPI.Models.DTOs;
using GrainOperationAPI.Models;

namespace GrainOperationAPI.Services
{
    public class PriceService
    {
        private readonly GrainOperationContext _context;

        public PriceService(GrainOperationContext context)
        {
            _context = context;
        }

        public async Task AddPriceAsync(PriceDto priceDto)
        {
            var price = new PriceModel
            {
                GrainType = priceDto.GrainType,
                GrainClass = priceDto.GrainClass,
                Price = priceDto.Price,
                Timestamp = DateTime.Now // Or UTC now if you prefer
            };

            _context.Prices.Add(price);
            await _context.SaveChangesAsync();
        }
    }

}
