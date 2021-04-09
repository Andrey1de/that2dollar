using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using that2dollar.Data;
using that2dollar.Models;

namespace that2dollar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class crudController : ControllerBase
    {
        private readonly ToUsdContext _context;

        public crudController(ToUsdContext context)
        {
            _context = context;
        }

        // GET: api/Crud
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RateToUsd>>> GetRates()
        {
            return await _context.Rates.ToListAsync();
        }

        // GET: api/Crud/5
        [HttpGet("{code}")]
        public async Task<ActionResult<RateToUsd>> GetRateToUsd(string code)
        {
            var rateToUsd = await _context.Rates.FindAsync(code.ToUpper());

            if (rateToUsd == null)
            {
                return NotFound();
            }

            return rateToUsd;
        }

        // PUT: api/Crud/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{code}")]
        public async Task<IActionResult> PutRateToUsd(string code, RateToUsd rateToUsd)
        {
            code = code.ToUpper();
            if (code != rateToUsd.code)
            {
                return BadRequest();
            }

            _context.Entry(rateToUsd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RateToUsdExists(code))
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

        // POST: api/Crud
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RateToUsd>> PostRateToUsd(RateToUsd rateToUsd)
        {
            _context.Rates.Add(rateToUsd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RateToUsdExists(rateToUsd.code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRateToUsd", new { code = rateToUsd.code }, rateToUsd);
        }

        // DELETE: api/Crud/5
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteRateToUsd(string code)
        {
            code = code.ToUpper();
            var rateToUsd = await _context.Rates.FindAsync(code);
            if (rateToUsd == null)
            {
                return NotFound();
            }

            _context.Rates.Remove(rateToUsd);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RateToUsdExists(string code)
        {
            return _context.Rates.Any(e => e.code == code);
        }
    }
}
