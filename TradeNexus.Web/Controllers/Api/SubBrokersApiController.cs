using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNexus.Web.Data;
using TradeNexus.Web.Models;

namespace TradeNexus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/subbrokers")]
    public class SubBrokersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubBrokersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Trader,Risk")]
        public async Task<ActionResult<IEnumerable<SubBroker>>> GetSubBrokers()
        {
            var subBrokers = await _context.SubBrokers
                .AsNoTracking()
                .ToListAsync();
            return Ok(subBrokers);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Trader,Risk")]
        public async Task<ActionResult<SubBroker>> GetSubBroker(int id)
        {
            var subBroker = await _context.SubBrokers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subBroker == null)
            {
                return NotFound();
            }

            return Ok(subBroker);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubBroker>> CreateSubBroker([FromBody] SubBroker subBroker)
        {
            _context.SubBrokers.Add(subBroker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubBroker), new { id = subBroker.Id }, subBroker);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubBroker(int id, [FromBody] SubBroker subBroker)
        {
            if (id != subBroker.Id)
            {
                return BadRequest("Mismatched sub-broker ID.");
            }

            _context.Entry(subBroker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SubBrokers.AnyAsync(s => s.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubBroker(int id)
        {
            var subBroker = await _context.SubBrokers.FindAsync(id);
            if (subBroker == null)
            {
                return NotFound();
            }

            _context.SubBrokers.Remove(subBroker);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
