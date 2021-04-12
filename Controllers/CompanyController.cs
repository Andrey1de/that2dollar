using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using that2dollar.Models;
using that2dollar.Services;
using That2Dollar.Models;

namespace that2dollar.Controllers
{
    /// <summary>
    /// API is designed to control and update the ratios 
    /// of currencies to USD 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {

        readonly IGlobalQuotesService SrvQuote;
        readonly ICompanyOverviewService SrvOverview;
        readonly ICompanyBestMatchesService SrvBestMatches;
        //  readonly HttpClient Client;

        //ToUsdContext Context;
        public CompanyController(
            IGlobalQuotesService srvQuotes,
             ICompanyOverviewService srvOverview,
           ICompanyBestMatchesService srvBestMatches
             )
        {

            //  Client = PrepareHttpClient(_httpClient); ;
            SrvQuote = srvQuotes;// = context;
            SrvOverview = srvOverview;// = context;
            SrvBestMatches = srvBestMatches;// = context;
                           //  Srv.Client = Client;//Important those client would be used for service


           // Task.Run(srv.TryInit).Wait();
        }



        /// <summary>
        /// Get  Global Quote for the last hour
        /// </summary>
        /// <param name="symbol">Company Symbol (IBM))</param>
        /// <returns>GlobalQuote</returns>
        // GET: api/GlobalQuote/5
        [HttpGet("GlobalQuote/{symbol}")]
        public async Task<ActionResult<GlobalQuote>> GetGlobalQuote(string symbol)
        {
            symbol = (symbol ?? "").ToUpper();
            var globalQuote = await SrvQuote.GetItem(symbol);

            if (globalQuote == null)
            {
                return NotFound();
            }

            return globalQuote;
        }


        /// <summary>
        /// Get  Best list of companies , matched by index by index,  actual for the last hour
        /// </summary>
        /// <param name="index">Company Symbol (TSLA)</param>
        /// <returns>CompanyBestMatches</returns>
        // GET: api Company/BestMatches/TSLA
        [HttpGet("BestMatches/{symbol}")]
        public async Task<ActionResult<Dictionary<string, CompanyMatch>>> GetBestMatches(string symbol)
        {
            symbol = (symbol ?? "").ToUpper();
            CompanyBestMatches bestMatches = await SrvBestMatches.GetItem(symbol);
            Dictionary<string, CompanyMatch> matches = bestMatches.BestMatches;

            if (bestMatches == null)
            {
                return NotFound();
            }

            return matches;
        }

        /// <summary>
        /// Get  Company overview by index
        /// </summary>
        /// <param name="index">Company symbol (TSLA)</param>
        /// <returns>CompanyOverview</returns>
        // GET: api Company/Overview/IBM
        [HttpGet("Overview/{symbol}")]
        public async Task<ActionResult<CompanyOverview>> GetOverview(string symbol)
        {
            symbol = (symbol ?? "").ToUpper();
            CompanyOverview overview = await SrvOverview.GetItem(symbol);

            if (overview == null)
            {
                return NotFound();
            }

            return overview;
        }

    }


}

