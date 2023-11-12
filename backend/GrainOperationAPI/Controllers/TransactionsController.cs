using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using GrainOperationAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly GrainOperationContext _context;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(GrainOperationContext context, ILogger<TransactionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TransactionModel>> GetTransactions()
        {
            return _context.Transactions.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionModel>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }
            return transaction;
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, TransactionModel transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }
            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.TransactionId == id)) 
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) 
            { 
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<TransactionModel>> PostTransaction([FromBody] TransactionDto transactionDto)
        {
            _logger.LogInformation("Received transaction for processing: {@TransactionDto}", transactionDto);

            // Attempt to retrieve the farmer from the database or create a new one if it doesn't exist
            var farmer = await _context.Farmers
                            .FirstOrDefaultAsync(f => f.FarmerFirstName == transactionDto.FarmerFirstName &&
                                                      f.FarmerLastName == transactionDto.FarmerLastName)
                            ?? new FarmerModel { FarmerFirstName = transactionDto.FarmerFirstName, FarmerLastName = transactionDto.FarmerLastName };

            // Attempt to retrieve the truck from the database or create a new one if it doesn't exist
            var truck = await _context.Trucks
                            .FirstOrDefaultAsync(t => t.TruckNumbers == transactionDto.TruckNumbers)
                            ?? new TruckModel { TruckNumbers = transactionDto.TruckNumbers, TruckStorage = transactionDto.TruckStorage, Farmer = farmer };

            // Create the transaction model from the DTO
            var transaction = new TransactionModel
            {
                Truck = truck,
                GrainType = transactionDto.Grain.Type,
                GrainClass = transactionDto.Grain.Class,
                Dryness = transactionDto.Grain.Dryness,
                Cleanliness = transactionDto.Grain.Cleanliness,
                GrainWeight = transactionDto.GrainWeight,
                ArrivalTime = DateTime.Parse(transactionDto.Arrival), // Consider using DateTime.TryParse to avoid exceptions
                WantedPay = transactionDto.WantedPay,
                PricePerTonne = transactionDto.PricePerTonne,
                // Status should be set based on business logic, possibly here or elsewhere before saving
            };

            // Add the transaction to the context
            _context.Transactions.Add(transaction);

            try
            {
                // Save changes to the context
                await _context.SaveChangesAsync();
                _logger.LogInformation("Transaction processed successfully: {@Transaction}", transaction);

                // Return the created transaction along with the '201 Created' response
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the transaction: {@TransactionDto}", transactionDto);
                // Return a user-friendly message with the '500 Internal Server Error' response
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
