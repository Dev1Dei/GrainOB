using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceHistoryController : ControllerBase
    {
        private readonly GrainOperationContext _context;

        public BalanceHistoryController(GrainOperationContext context)
        {
            _context = context;
        }

        // GET: api/BalanceHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalanceHistoryModel>>> GetBalanceHistory()
        {
            return await _context.BalanceHistory.ToListAsync();
        }

        // POST: api/BalanceHistory
        [HttpPost]
        public async Task<ActionResult<BalanceHistoryModel>> PostBalanceHistory([FromBody] BalanceHistoryModel balanceHistory)
        {
            if (balanceHistory == null)
            {
                return BadRequest();
            }

            _context.BalanceHistory.Add(balanceHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBalanceHistory", new { id = balanceHistory.HistoryId }, balanceHistory);


        }
    }
}
