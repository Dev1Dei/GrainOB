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
    public class FarmersController : ControllerBase
    {
        private readonly GrainOperationContext _context;

        public FarmersController(GrainOperationContext context)
        {
            _context = context;
        }

        // GET: api/farmers
        [HttpGet]
        public ActionResult<IEnumerable<FarmerModel>> GetFarmers()
        {
            return _context.Farmers.ToList();
        }

        // GET: api/farmers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FarmerModel>> GetFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            return farmer;
        }

        // POST: api/farmers
        [HttpPost]
        public async Task<ActionResult<FarmerModel>> PostFarmer(FarmerModel farmer)
        {
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFarmer", new { id = farmer.FarmerId }, farmer);
        }

        // PUT: api/farmers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFarmer(int id, FarmerModel farmer)
        {
            if (id != farmer.FarmerId)
            {
                return BadRequest();
            }

            _context.Entry(farmer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Farmers.Any(e => e.FarmerId == id))
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

        // DELETE: api/farmers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarmer(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }

            _context.Farmers.Remove(farmer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
