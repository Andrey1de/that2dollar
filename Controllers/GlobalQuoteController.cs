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
    public class globalQuoteController : ControllerBase
    {

        readonly IGlobalQuotesService Srv;
      //  readonly HttpClient Client;

        //ToUsdContext Context;
        public globalQuoteController(
            IGlobalQuotesService srv)
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
        // GET: api/GlobalQuote
        [HttpGet]
        public ActionResult<GlobalQuote[]> GetRates()
        {
            return Ok(Srv.AllData);
        }

        /// <summary>
        /// Get the ratio of Global Quote to USD actual for the last hour
        /// </summary>
        /// <param name="symbol"> Global Quote symbol (ILS)</param>
        /// <returns></returns>
        // GET: api/GlobalQuote/5
        [HttpGet("{symbol}")]
        public async Task<ActionResult<GlobalQuote>> GetGlobalQuote(string symbol)
        {
            symbol = (symbol ?? "").ToUpper();
            var globalQuote = await Srv.GetItem(symbol);

            if (globalQuote == null)
            {
                return NotFound();
            }

            return globalQuote;
        }

         
        /// <summary>
        /// Remove ratio from service 
        /// </summary>
        /// <param name="symbol"> Global Quote symbol - 4 letters for example EUR</param>
        /// <returns></returns>
        [HttpDelete("{symbol}")]
        public async Task<IActionResult> DeleteGlobalQuote(string symbol)
        {
            symbol = (symbol ?? "").ToUpper();
            var b = await Srv.RemoveItem(symbol);
            if (!b)
            {
                return NotFound();
            }


            return Ok(symbol);
        }

    }
}
