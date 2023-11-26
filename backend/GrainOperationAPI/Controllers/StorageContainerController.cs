using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GrainOperationAPI.Models.DTOs;
using GrainOperationAPI.Services;
using System.Net.Http;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageContainerController : ControllerBase
    {
        private readonly GrainOperationContext _context;
        private readonly PriceService _priceService;
        private readonly HttpClient _httpClient;

        public StorageContainerController(GrainOperationContext context, PriceService priceService, HttpClient httpClient)
        {
            _context = context;
            _priceService = priceService;
            _httpClient = httpClient;
        }

        // GET: api/StorageContainer/{containerId}
        [HttpGet("{containerId}")]
        public async Task<ActionResult<StorageContainerModel>> GetContainer(int containerId)
        {
            var container = await _context.StorageContainers.FindAsync(containerId);
            if (container == null)
            {
                return NotFound("Container not found.");
            }

            return Ok(container);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<StorageContainerModel>>> GetContainersByUserId(int userId)
        {
            var containers = await _context.StorageContainers
                                            .Where(c => c.UserId == userId)
                                            .ToListAsync();

            if (!containers.Any())
            {
                return NotFound("No containers found for the given user.");
            }

            return containers;
        }
        // PUT: api/StorageContainer/{containerId}/clean
        [HttpPut("{containerId}/clean")]
        public async Task<IActionResult> CleanContainer(int containerId)
        {
            var container = await _context.StorageContainers.FindAsync(containerId);
            if (container == null)
            {
                return NotFound();
            }

            container.Cleanliness = 100m; // Assuming 100 is the max cleanliness value
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{containerId}/clear")]
        public async Task<IActionResult> ClearContainer(int containerId)
        {
            var userId = 1;
            var container = await _context.StorageContainers.FindAsync(containerId);
            if (container == null)
            {
                return NotFound();
            }

            container.Cleanliness = 0m; // Assuming 100 is the max cleanliness value
            container.Dryness = 0m;
            container.FreeSpace = container.TotalCapacity;
            container.GrainClass = "None";
            container.GrainType = "None";
            container.Weight = 0;
            container.StoredSpace = 0;
            container.UserId = userId;
            container.Name = container.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/StorageContainer/{containerId}/dry
        [HttpPut("{containerId}/dry")]
        public async Task<IActionResult> DryContainer(int containerId)
        {
            var container = await _context.StorageContainers.FindAsync(containerId);
            if (container == null)
            {
                return NotFound();
            }

            container.Dryness = 100m;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/StorageContainer/{containerId}/average
        [HttpPut("{containerId}/average")]
        public async Task<IActionResult> AverageContainer(int containerId, [FromBody] StorageContainerModel updatedContainer)
        {
            var container = await _context.StorageContainers.FindAsync(containerId);
            if (container == null)
            {
                return NotFound();
            }

            // Calculate the total weight after adding the new grain
            var totalWeight = container.StoredSpace + updatedContainer.Weight;

            // Check if the container can hold the new grain
            if (totalWeight > container.TotalCapacity)
            {
                return BadRequest("Not enough space in the container for the given weight.");
            }

            // Perform weighted average calculations for dryness and cleanliness
            if (totalWeight > 0) // Prevent division by zero
            {
                container.Dryness = ((container.Dryness * container.StoredSpace) +
                                     (updatedContainer.Dryness * updatedContainer.Weight)) / totalWeight;

                container.Cleanliness = ((container.Cleanliness * container.StoredSpace) +
                                         (updatedContainer.Cleanliness * updatedContainer.Weight)) / totalWeight;
            }

            // Update the container's stored space and free space
            container.StoredSpace = totalWeight;
            container.FreeSpace = container.TotalCapacity - totalWeight;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<StorageContainerModel>> PostContainer([FromBody] StorageContainerDto dto)
        {
            var userId = 1; // Replace with actual user id from the logged-in user context

            // Check if UserBalance exists for the given UserId to avoid ForeignKey Constraint error.
            var userBalanceExists = await _context.UserBalances.AnyAsync(ub => ub.UserId == userId);
            if (!userBalanceExists)
            {
                return BadRequest("Invalid UserId: UserBalance not found.");
            }

            var newContainer = new StorageContainerModel
            {
                UserId = userId,
                GrainType = dto.GrainType ?? "None", // Set default value if not provided
                GrainClass = dto.GrainClass ?? "None", // Set default value if not provided
                TotalCapacity = dto.TotalCapacity,
                FreeSpace = dto.TotalCapacity, // Assuming the new container starts empty
                StoredSpace = 0, // Assuming the new container starts empty
                Dryness = 0, // Assuming default value
                Cleanliness = 0, // Assuming default value
                Name = dto.Name,
            };

            _context.StorageContainers.Add(newContainer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContainer), new { containerId = newContainer.ContainerId }, newContainer);
        }
    }
}