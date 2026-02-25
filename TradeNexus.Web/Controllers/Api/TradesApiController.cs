using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNexus.Web.Data;
using TradeNexus.Web.Models;

namespace TradeNexus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/trades")]
    public class TradesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TradesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Trader,Risk")]
        public async Task<ActionResult<IEnumerable<Trade>>> GetTrades([FromQuery] int? clientId, [FromQuery] int? subBrokerId)
        {
            var query = _context.Trades.AsNoTracking().AsQueryable();

            if (clientId.HasValue)
            {
                query = query.Where(t => t.ClientId == clientId.Value);
            }

            if (subBrokerId.HasValue)
            {
                query = query.Where(t => t.SubBrokerId == subBrokerId.Value);
            }

            var trades = await query
                .OrderByDescending(t => t.TradeDate)
                .ToListAsync();

            return Ok(trades);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Trader,Risk")]
        public async Task<ActionResult<Trade>> GetTrade(int id)
        {
            var trade = await _context.Trades
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TradeId == id);

            if (trade == null)
            {
                return NotFound();
            }

            return Ok(trade);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Trade>> CreateTrade([FromBody] Trade trade)
        {
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrade), new { id = trade.TradeId }, trade);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] Trade trade)
        {
            if (id != trade.TradeId)
            {
                return BadRequest("Mismatched trade ID.");
            }

            _context.Entry(trade).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Trades.AnyAsync(t => t.TradeId == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
            {
                return NotFound();
            }

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
