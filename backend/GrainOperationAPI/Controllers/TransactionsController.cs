using GrainOperationAPI.Data;
using GrainOperationAPI.Hubs;
using GrainOperationAPI.Models;
using GrainOperationAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly GrainOperationContext _context;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TransactionsController(GrainOperationContext context, ILogger<TransactionsController> logger, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
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
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
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
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] UpdateTransactionStatusDto statusDto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Status = statusDto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionModel>> PostTransaction([FromBody] TransactionDto transactionDto)
        {
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.FarmerFirstName == transactionDto.FarmerFirstName &&
                                          f.FarmerLastName == transactionDto.FarmerLastName)
                ?? new FarmerModel { FarmerFirstName = transactionDto.FarmerFirstName, FarmerLastName = transactionDto.FarmerLastName };

            var truck = await _context.Trucks
                .FirstOrDefaultAsync(t => t.TruckNumbers == transactionDto.TruckNumbers)
                ?? new TruckModel { TruckNumbers = transactionDto.TruckNumbers, TruckStorage = transactionDto.TruckStorage, Farmer = farmer };

            var transaction = new TransactionModel
            {
                Truck = truck,
                GrainType = transactionDto.Grain.Type,
                GrainClass = transactionDto.Grain.Class,
                Dryness = transactionDto.Grain.Dryness,
                Cleanliness = transactionDto.Grain.Cleanliness,
                GrainWeight = transactionDto.GrainWeight,
                ArrivalTime = DateTime.Parse(transactionDto.Arrival),
                WantedPay = transactionDto.WantedPay,
                PricePerTonne = transactionDto.PricePerTonne,
                Status = "Pending"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var creationEvent = new
            {
                Farmer = new { farmer.FarmerFirstName, farmer.FarmerLastName },
                Truck = new { truck.TruckNumbers, truck.TruckStorage },
                Transaction = new
                {
                    transaction.TransactionId,
                    transaction.GrainType,
                    transaction.GrainClass,
                    Dryness = transaction.Dryness,
                    Cleanliness = transaction.Cleanliness,
                    GrainWeight = transaction.GrainWeight,
                    ArrivalTime = transaction.ArrivalTime,
                    WantedPay = transaction.WantedPay,
                    PricePerTonne = transaction.PricePerTonne
                }
            };
            _logger.LogInformation($"!!!!!!!!!!!!!!!!!!!Sending message: {creationEvent}");
            await _hubContext.Clients.All.SendAsync("CreationEventOccurred", creationEvent);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
        }
    }
}
