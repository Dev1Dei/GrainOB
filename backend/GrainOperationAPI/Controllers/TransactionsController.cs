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
        [HttpGet("pending")]
        public async Task<ActionResult> GetPendingTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            int totalRecords = await _context.Transactions.CountAsync(t => t.Status == "Pending");
            var transactions = await _context.Transactions
                .Where(t => t.Status == "Pending")
                .Include(t => t.Truck)
                    .ThenInclude(tr => tr.Farmer)
                .Select(t => new
                 {
                    t.TransactionId,
                    t.GrainType,
                    t.GrainClass,
                    t.Dryness,
                    t.Cleanliness,
                    t.WantedPay,
                    t.ArrivalTime,
                    t.PricePerTonne,
                    TruckNumbers = t.Truck.TruckNumbers,
                    TruckStorage = t.Truck.TruckStorage,
                    FarmerFirstName = t.Truck.Farmer.FarmerFirstName,
                    FarmerLastName = t.Truck.Farmer.FarmerLastName,
                 })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Transactions = transactions });
        }
        [HttpGet("AcceptedOrDenied")]
        public async Task<ActionResult> GetAcceptedorDeniedTransactions(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string sort = "asc"
       )
        {
            var transactionsQuery = _context.Transactions
                .Where(t => t.Status == "Accepted" || t.Status == "Denied")
                .Include(t => t.Truck)
                    .ThenInclude(tr => tr.Farmer);

            // Apply sorting based on the 'sort' parameter
            var orderedTransactionsQuery = sort == "asc"
                ? transactionsQuery.OrderBy(t => t.TransactionId)
                : transactionsQuery.OrderByDescending(t => t.TransactionId);

            int totalRecords = await orderedTransactionsQuery.CountAsync();

            var transactions = await orderedTransactionsQuery
                .Select(t => new
                {
                    t.TransactionId,
                    t.GrainType,
                    t.GrainClass,
                    t.Dryness,
                    t.Cleanliness,
                    t.WantedPay,
                    t.ArrivalTime,
                    t.PricePerTonne,
                    t.Status,
                    TruckNumbers = t.Truck.TruckNumbers,
                    TruckStorage = t.Truck.TruckStorage,
                    FarmerFirstName = t.Truck.Farmer.FarmerFirstName,
                    FarmerLastName = t.Truck.Farmer.FarmerLastName,
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Transactions = transactions });
        }

        // This action will handle requests like: GET api/transactions/pending/count
        [HttpGet("pending/count")]
        public async Task<ActionResult<int>> GetPendingTransactionsCount()
        {
            return await _context.Transactions.CountAsync(t => t.Status == "Pending");
        }
        [HttpGet("AoD/count")]
        public async Task<ActionResult<int>> GetAoDTransactionsCount()
        {
            return await _context.Transactions.CountAsync(t => t.Status == "Accepted" || t.Status == "Denied");
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
                Dryness = (decimal)transactionDto.Grain.Dryness,
                Cleanliness = (decimal)transactionDto.Grain.Cleanliness,
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
            _logger.LogInformation($"Sending message: {creationEvent}");
            await _hubContext.Clients.All.SendAsync("CreationEventOccurred", creationEvent);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
        }
        [HttpPut("{transactionId}/assign-container")]
        public async Task<IActionResult> AssignTransactionToContainer(int transactionId, [FromBody] AssignContainerDto dto)
        {
            _logger.LogInformation($"Raw request body: {await new StreamReader(Request.Body).ReadToEndAsync()}");

            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found.");
                return NotFound("Transaction not found.");
            }

            var container = await _context.StorageContainers.FindAsync(dto.ContainerId);
            if (container == null)
            {
                _logger.LogWarning($"Container with ID {dto.ContainerId} not found.");
                return NotFound("Container not found.");
            }

            if (!IsContainerSuitableForTransaction(container, transaction))
            {
                _logger.LogWarning($"Container {dto.ContainerId} is not suitable for transaction {transactionId}.");
                return BadRequest("The container is not suitable for the transaction.");
            }

            transaction.ContainerId = dto.ContainerId;
            _logger.LogInformation($"Assigning container {dto.ContainerId} to transaction {transactionId}.");
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Container {dto.ContainerId} has been successfully assigned to transaction {transactionId}.");

            return NoContent();
        }


        private bool IsContainerSuitableForTransaction(StorageContainerModel container, TransactionModel transaction)
        {
            decimal transactionGrainWeight = (decimal)transaction.GrainWeight;
            bool hasEnoughSpace = container.FreeSpace >= transactionGrainWeight;

            // Check if the container is empty (has not been used before)
            bool isContainerEmpty = container.GrainType == "None" && container.StoredSpace == 0.00M;

            bool matchesGrainType = isContainerEmpty || container.GrainType == transaction.GrainType || container.GrainType == "Any";
            bool matchesGrainClass = isContainerEmpty || container.GrainClass == transaction.GrainClass || container.GrainClass == "None" || transaction.GrainClass == "None";

            return hasEnoughSpace && matchesGrainType && matchesGrainClass;
        }


        [HttpPost("{transactionId}/complete")]
        public async Task<IActionResult> CompleteTransaction(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Truck)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                _logger.LogError($"Transaction with ID {transactionId} not found.");
                return NotFound();
            }

            var container = await _context.StorageContainers
                .FirstOrDefaultAsync(c => c.ContainerId == transaction.ContainerId);

            if (container == null)
            {
                _logger.LogError($"Container with ID {transaction.ContainerId} not found.");
                return NotFound();
            }

            // Check if the container has enough space
            var totalWeightAfterTransaction = container.Weight + (decimal)transaction.GrainWeight;
            if (totalWeightAfterTransaction > container.TotalCapacity)
            {
                _logger.LogError($"Not enough space in container {container.ContainerId} for transaction {transactionId}.");
                return BadRequest("Not enough space in the container.");
            }

            // If there was grain in the container before, calculate the weighted average
            if (container.Weight > 0)
            {
                var totalDryness = (container.Dryness * container.Weight) + (transaction.Dryness * transaction.GrainWeight);
                container.Dryness = totalDryness / totalWeightAfterTransaction;

                var totalCleanliness = (container.Cleanliness * container.Weight) + (transaction.Cleanliness * transaction.GrainWeight);
                container.Cleanliness = totalDryness / totalWeightAfterTransaction;
            }
            else
            {
                container.Dryness = (decimal)transaction.Dryness;
                container.Cleanliness = (decimal)transaction.Cleanliness;
            }

            // Perform the update here
            container.GrainType = transaction.GrainType;
            container.GrainClass = transaction.GrainClass ?? "None";
            container.Weight = totalWeightAfterTransaction;
            container.FreeSpace = container.TotalCapacity - container.Weight;
            container.StoredSpace = container.Weight; // Assuming StoredSpace is the occupied space.

            // Assuming 'Name' is the identifier for the grain's lot or some kind of identifier you can construct.
            container.Name = container.Name;

            _context.StorageContainers.Update(container);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transaction {transactionId} has been completed and container {container.ContainerId} updated.");

            return Ok();
        }

    }
}
