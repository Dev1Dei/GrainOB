using GrainOperationAPI.Models.DTOs;
using GrainOperationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GrainOperationAPI.Controllers
{
    public class PriceController : ControllerBase
    {
        private readonly PriceService _priceService;

        public PriceController(PriceService priceService)
        {
            _priceService = priceService;
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


    }
}
