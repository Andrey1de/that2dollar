using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using that2dollar.Data;
using that2dollar.Models;
using that2dollar.Services;

namespace that2dollar.Controllers
{
    /// <summary>
    /// API is designed to control and update the ratios 
    /// of currencies to USD 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class todollarController : ControllerBase
    {

        IRatesService Srv;
        //ToUsdContext Context;
        public todollarController(//ToUsdContext context,
            IRatesService srv)
        {
            Srv = srv;// = context;
                      // Srv.Context = Context = context;
            Task.Run(srv.TryInit).Wait();
        }

        /// <summary>
        /// Get a list of the of currencies to USD ratio 
        /// </summary>
        /// <returns></returns>
        // GET: api/RateToUsd
        [HttpGet]
        public ActionResult<RateToUsd[]> GetRates()
        {
            return Ok(Srv.Rates);
        }

        /// <summary>
        /// Get the ratio of currency to USD actual for the last hour
        /// </summary>
        /// <param name="code"> currency code (ILS)</param>
        /// <returns></returns>
        // GET: api/RateToUsd/5
        [HttpGet("{code}")]
        public async Task<ActionResult<RateToUsd>> GetRateToUsd(string code)
        {
            code = (code ?? "").ToUpper();
            var rateToUsd = await Srv.GetRatio(code);

            if (rateToUsd == null)
            {
                return NotFound();
            }

            return rateToUsd;
        }

        /// <summary>
        /// Get the ratio of two currencies From / To actual for the last hour
        /// </summary>
        /// <param name="from"> currency code (EUR)</param>
        /// <param name="to"> currency code (JPY)</param>
        /// <returns></returns>
        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<FromTo>>
                GetRatioForPair(string from, string to)
        {
            from = (from ?? "").ToUpper();
            to = (to ?? "").ToUpper();

            var rateToUsd = await Srv.GetRatioForPair(from, to);

            if (rateToUsd == null )
            {
                return NotFound();
            }

            return rateToUsd;
        }
        /// <summary>
        /// Remove ratio from service 
        /// </summary>
        /// <param name="code"> currency code - 3 letters for example EUR</param>
        /// <returns></returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteRateToUsd(string code)
        {
            code = (code ?? "").ToUpper();
            var b = await Srv.Remove(code);
            if (!b)
            {
                return NotFound();
            }


            return Ok(code);
        }

        //private bool RateToUsdExists(string id)
        //{
        //    return _context.Rates.Any(e => e.code == id);
        //}
        /////// <summary>
        ///// 
        ///// </summary>
        ///// <param name="code"></param>
        ///// <param name="rateToUsd"></param>
        ///// <returns></returns>
        //// PUT: api/RateToUsd/RUB
        //[HttpPut("{code}")]
        //public async Task<IActionResult> PutRateToUsd(string code, RateToUsd rateToUsd)
        //{
        //    if (code != rateToUsd.code)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(rateToUsd).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RateToUsdExists(code))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/RateToUsd
        //[HttpPost]
        //public async Task<ActionResult<RateToUsd>> PostRateToUsd(RateToUsd rateToUsd)
        //{
        //    _context.Rates.Add(rateToUsd);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (RateToUsdExists(rateToUsd.code))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetRateToUsd", new { id = rateToUsd.code }, rateToUsd);
        //}

        //// DELETE: api/RateToUsd/RUB

    }
}
