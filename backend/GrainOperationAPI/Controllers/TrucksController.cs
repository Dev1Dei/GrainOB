using Microsoft.AspNetCore.Mvc;
using GrainOperationAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using GrainOperationAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrucksController : ControllerBase
    {
        private readonly GrainOperationContext _context;

        public TrucksController(GrainOperationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TruckModel>> GetTrucks()
        {
            return _context.Trucks.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TruckModel>> GetTruck(int id)
        {
            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null)
            {
                return NotFound();
            }
            return truck;
        }

        [HttpPost]
        public async Task<ActionResult<TruckModel>> PostTruck(TruckModel truck)
        {
            _context.Trucks.Add(truck);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTruck", new { id = truck.TruckId }, truck);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTruck(int id, TruckModel truck)
        {
            if (id != truck.TruckId)
            {
                return BadRequest();
            }

            _context.Entry(truck).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Trucks.Any(e => e.TruckId == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruck(int id)
        {
            var truck = await _context.Trucks.FindAsync(id);
            if(truck == null)
            {
                return NotFound();
            }

            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
