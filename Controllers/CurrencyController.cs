using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class CurrencyController : ControllerBase
    {

        readonly IRatesService Srv;
        //  readonly HttpClient Client;

        //ToUsdContext Context;
        public CurrencyController(
            IRatesService srv)
        {

            //  Client = PrepareHttpClient(_httpClient); ;
            Srv = srv;// = context;
                      //  Srv.Client = Client;//Important those client would be used for service


            Task.Run(srv.TryInit).Wait();
        }


        /// <summary>
        /// Get a list of the of currencies to USD ratio 
        /// </summary>
        /// <returns></returns>
        // GET: api/RateToUsd
        [HttpGet("RateToUsd/")]
        public ActionResult<RateToUsd[]> GetRates()
        {
            return Ok(Srv.AllData);
        }

        /// <summary>
        /// Get the ratio of currency to USD actual for the last hour
        /// </summary>
        /// <param name="code"> currency Code (ILS)</param>
        /// <returns></returns>
        // GET: api/RateToUsd/5
        [HttpGet("RateToUsd/{code}")]
        public async Task<ActionResult<RateToUsd>> GetRateToUsd(string code)
        {
            code = (code ?? "").ToUpper();
            var rateToUsd = await Srv.GetItem(code);

            if (rateToUsd == null)
            {
                return NotFound();
            }

            return rateToUsd;
        }

        /// <summary>
        /// Get the ratio of two currencies From / To actual for the last hour
        /// </summary>
        /// <param name="from"> currency Code (EUR)</param>
        /// <param name="to"> currency Code (JPY)</param>
        /// <returns></returns>
        [HttpGet("ToUsd/{from}/{to}")]
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
        /// <param name="code"> currency Code - 3 letters for example EUR</param>
        /// <returns></returns>
        [HttpDelete("RateToUsd/{code}")]
        public  ActionResult DeleteRateToUsd(string code)
        {
            code = (code ?? "").ToUpper();
            var item =  Srv.RemoveItem(code);
            if (item == null)
            {
                return NotFound();
            }


            return Ok(code);
        }

        //private bool RateToUsdExists(string id)
        //{
        //    return _context.Rates.Any(e => e.Code == id);
        //}
        /////// <summary>
        ///// 
        ///// </summary>
        ///// <param name="Code"></param>
        ///// <param name="rateToUsd"></param>
        ///// <returns></returns>
        //// PUT: api/RateToUsd/RUB
        //[HttpPut("{Code}")]
        //public async Task<IActionResult> PutRateToUsd(string Code, RateToUsd rateToUsd)
        //{
        //    if (Code != rateToUsd.Code)
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
        //        if (!RateToUsdExists(Code))
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
        //        if (RateToUsdExists(rateToUsd.Code))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetRateToUsd", new { id = rateToUsd.Code }, rateToUsd);
        //}

        //// DELETE: api/RateToUsd/RUB

    }
}
