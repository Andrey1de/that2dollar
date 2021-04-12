using that2dollar.Data;
using that2dollar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using That2Dollar.Models;

namespace that2dollar.Services
{



    public interface ICompanyBestMatchesService : ISpooledService<CompanyBestMatches>
    {
    }
    public class CompanyBestMatchesService :
        AbstractRemoteService<CompanyBestMatches>, ICompanyBestMatchesService
    {
        public override string ConvertorUrl =>
             "https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=";

        public override string GetKey(CompanyBestMatches item) => "Keyword";


        public CompanyBestMatchesService(HttpClient _httpClient,
                           ILogger<CompanyBestMatchesService> logger)
            : base(_httpClient, logger, "CompanyBestMatches")
        {

        }
        public override CompanyBestMatches DecodeBody(string keyword, string jsonBody)
        {

            try
            {
                CompanyBestMatches ret = FromJson(jsonBody);
                ret.Keyword = keyword;
                   return ret;
            }
            catch (Exception ex)
            {

                LogError(ex);
            }
            return null;
        }
        private CompanyBestMatches FromJson(string json)
        {
            JObject o = JObject.Parse(json);
            var dic = o["bestMatches"].ToArray();
            CompanyBestMatches that = new CompanyBestMatches();

            //CompanyBestMatches that = (CompanyBestMatches)o.ToObject(typeof(CompanyBestMatches));
            foreach (var token in dic)
            {
                var item = FromToken(token);
                that.BestMatches[item.Symbol] = item;

            }
            return that;

        }
        private CompanyMatch FromToken(JToken obj)
        {
            CompanyMatch that = new CompanyMatch();

            that.Symbol = obj["1. symbol"].ToString();// 'TSCO.LON',
            that.Name = obj["2. name"].ToString();// 'Tesco PLC',
            that.Type = obj["3. type"].ToString();// 'Equity',
            that.Region = obj["4. region"].ToString();// 'United Kingdom',
            that.MarketOpen = obj["5. marketOpen"].ToString();// '08:00',
            that.MarketClose = obj["6. marketClose"].ToString();// '16:30',
            that.Timezone = obj["7. timezone"].ToString();// 'UTC+01',
            that.Currency = obj["8. currency"].ToString();// 'GBP',
            that.MatchScore = (double)obj["9. matchScore"].ToObject(typeof(double));// '0.7273'
                                                                                    //
            return that;
        }


    }


}



