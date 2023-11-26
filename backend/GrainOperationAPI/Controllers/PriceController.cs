using GrainOperationAPI.Data;
using GrainOperationAPI.Models.DTOs;
using GrainOperationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly PriceService _priceService;
        private readonly GrainOperationContext _context;
        private readonly ILogger<PriceController> _logger;

        public PriceController(PriceService priceService, GrainOperationContext context, ILogger<PriceController> logger)
        {
            _priceService = priceService;
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostPrices([FromBody] List<PriceDto> priceDtos)
        {
            foreach (var priceDto in priceDtos)
            {
                await _priceService.AddPriceAsync(priceDto);
            }
            return Ok();
        }
        [HttpGet("GetLatestPrice")]
        public async Task<IActionResult> GetLatestPrice([FromQuery] string grainType, [FromQuery] string grainClass)
        {
            try
            {
                var pricesQuery = _context.Prices.AsQueryable();

                if (!string.IsNullOrEmpty(grainType))
                {
                    pricesQuery = pricesQuery.Where(p => p.GrainType == grainType);
                }

                if (!string.IsNullOrEmpty(grainClass) && grainClass != "None")
                {
                    pricesQuery = pricesQuery.Where(p => p.GrainClass == grainClass);
                }

                // Get the latest price
                var latestPrice = await pricesQuery.OrderByDescending(p => p.Timestamp)
                                                   .FirstOrDefaultAsync();

                if (latestPrice == null)
                {
                    return NotFound("Price not found.");
                }

                return Ok(latestPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the latest price.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetPrices")]
        public async Task<IActionResult> GetPrices([FromQuery] string grainType, [FromQuery] string grainClass)
        {
            try
            {
                var pricesQuery = _context.Prices.AsQueryable();

                if (!string.IsNullOrEmpty(grainType))
                {
                    pricesQuery = pricesQuery.Where(p => p.GrainType == grainType);
                }

                if (!string.IsNullOrEmpty(grainClass) && grainClass != "None")
                {
                    pricesQuery = pricesQuery.Where(p => p.GrainClass == grainClass);
                }

                // Get the last 10 prices
                var lastTenPrices = await pricesQuery.OrderByDescending(p => p.Timestamp)  // Assuming Timestamp or another unique field indicates the order
                                                    .Take(10)
                                                    .ToListAsync();

                // If you want them in ascending order
                lastTenPrices = lastTenPrices.OrderBy(p => p.Timestamp).ToList();

                return Ok(lastTenPrices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching prices.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
