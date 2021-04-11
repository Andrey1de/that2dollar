using that2dollar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace that2dollar.Services
{



    public interface IGlobalQuotesService : ISpooledService<GlobalQuote>
    {
    }

    public class GlobalQuotesService :
        AbstractRemoteService<GlobalQuote>, IGlobalQuotesService
    {
        public override string ConvertorUrl =>
            "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=";


        public GlobalQuotesService(HttpClient _httpClient,
                           ILogger<GlobalQuotesService> logger)
            : base(_httpClient, logger, "GlobalQuote")
        {

        }


        public override GlobalQuote DecodeBody(string symbol, string jsonBody)
        {
            if (!jsonBody.Contains("Global Quote"))
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }
            //            JObject o = JObject.Parse(@"{
            //    'Global Quote': {
            //        '01. symbol': 'IBM',
            //        '02. open': '134.8700',
            //        '03. high': '135.7400',
            //        '04. low': '134.7100',
            //        '05. price': '135.7300',
            //        '06. volume': '3023916',
            //        '07. latest trading day': '2021-04-09',
            //        '08. previous close': '135.1200',
            //        '09. change': '0.6100',
            //        '10. change percent': '0.4515%'
            //    }
            //}");

            JObject o1 = JObject.Parse(jsonBody);
            if (o1 == null)
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }

            JToken o = o1["Global Quote"];
     
            if (o == null)
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }

            Func<string, string> fs = (string name) => (string)o[name] ?? "";
            Func<string, int> fi = (string name) => int.Parse((string)o[name] ?? "0");
            Func<string, double> fd = (string name) => double.Parse((string)o[name] ?? "0.0");


            GlobalQuote ret = new GlobalQuote();
            ret.Symbol = fs("01. symbol");//= "";// "IBM";//01

            if (string.IsNullOrWhiteSpace(ret.Symbol)
                || ret.Symbol != symbol)
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }

            ret.Open = fd("02. open");//= 0;// "133.7600";//02
            ret.High = fd("03. high");//= 0;// "133.9300";//03
            ret.Low = fd("04. low");//= 0;// "132.2700";//04
            ret.Price = fd("05. price");//= 0;//  integer "133.2300";//05
            ret.Volume = fi("06. volume");//= 0;// integer "4074161";//06
            ret.LatestTradingDay = fs("07. latest trading day");//= "";// "2021-04-01";//07
            ret.PreviousClose = fd("08. previous close");//= 0;// "133.2600";//08
            ret.Change = fd("09. change");//= 0;// "-0.0300";//09
            ret.ChangePercent = fs("10. change percent");//= 0;// "-0.0225%";//10
            ret.Updated = DateTime.Now;


            return ret;
        }

     


        public override SpoolItem<GlobalQuote> ToSpoolItem(GlobalQuote item)
        {
            return new SpoolItem<GlobalQuote>()
            {
                Key = item.Symbol,
                Data = item,
                ActualUntil = DateTime.Now.AddSeconds(MaxReadDelaySec)

            };
        }


    }

}

