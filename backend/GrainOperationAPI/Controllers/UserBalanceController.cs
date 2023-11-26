using GrainOperationAPI.Data;
using GrainOperationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GrainOperationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserBalanceController : ControllerBase
    {
        private readonly GrainOperationContext _context;

        public UserBalanceController(GrainOperationContext context)
        {
            _context = context;
        }

        // GET: api/UserBalance
        [HttpGet]
        public async Task<ActionResult<UserBalanceModel>> GetBalance()
        {
            var userBalance = await _context.UserBalances.FirstOrDefaultAsync();
            if (userBalance == null)
            {
                return NotFound("User balance not found.");
            }

            return userBalance;
        }

        // PUT: api/UserBalance
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateBalance(int userId, [FromBody] UserBalanceModel balanceUpdate)
        {
            var userBalance = await _context.UserBalances.FindAsync(userId);
            if (userBalance == null)
            {
                return NotFound("User balance not found.");
            }

            userBalance.Balance = balanceUpdate.Balance;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserBalances.Any(e => e.UserId == userId))
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
        [HttpPost("initialize")]
        public async Task<ActionResult<UserBalanceModel>> InitializeBalance()
        {
            if (await _context.UserBalances.AnyAsync())
            {
                return BadRequest("Balance has already been initialized.");
            }

            var userBalance = new UserBalanceModel { Balance = 0 }; // Set an initial balance, if needed.
            _context.UserBalances.Add(userBalance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBalance), new { userBalance.UserId }, userBalance);
        }
    }
}
