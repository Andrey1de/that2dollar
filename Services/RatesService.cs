using that2dollar.Data;
using that2dollar.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Net;
using that2dollar.Models;
using that2dollar.Data;
using System.Net.Http.Headers;

namespace that2dollar.Services
{


    public class RatesServiceDecoder : ISpoolDecoder
    {
        private readonly ILogger<RatesService> Log;

        readonly string Code;
     
        public RatesServiceDecoder(string _code , ILogger<RatesService> _log)
        {
            Code = _code;
            Log = _log;
        }
        public object DecodeBody(string jsonBody)
        {
            if (!jsonBody.Contains("1. From_Currency Code"))
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }
            string[] pars1 = new string[] {
                    "\"3. To_Currency Code\"",
                    "\"4. To_Currency Name\":",
                    "\"5. Exchange Rate\":",
                    "\"6. Last Refreshed\":",
                    "\"8. Bid Price\":" ,
                    "\"9. Ask Price\":"};

            string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);

            var code1 = f1(arrr[1]).ToUpper();
            if (arrr == null || arrr.Length < 7 || code1 != Code)
            {
                Log.LogWarning($"DecodeBody : Error of Parsing= [{arrr}]");
                return null;
            }

            RateToUsd ret = new RateToUsd();
            ret.code = Code;


            if (string.IsNullOrWhiteSpace(ret.code)
                || ret.code != Code)
            {
                return null;
            }

            ret.name = f1(arrr[2]);
            ret.rate = Double.Parse(f1(arrr[3]));
            ret.lastRefreshed = DateTime.Parse(f1(arrr[4]));
            ret.ask = Double.Parse(f1(arrr[5]));
            ret.bid = Double.Parse(f1(arrr[6]));
            ret.stored = DateTime.Now;


            return ret;
        }

        string f1(string str)
        {
            str = (str ?? "").Trim();
            var strRet = str.Split("\"".ToCharArray(),
                 StringSplitOptions.RemoveEmptyEntries)?[0];
            return strRet ?? "";
        }


    }
    public interface IRatesService
    {
        string ConvertorName { get; }
        Task TryInit();
        RateToUsd[] Rates { get; }
        Task<RateToUsd> GetRatio(string code);
        Task<FromTo> GetRatioForPair(string from, string to);

        Task<bool> Remove(string code);
    }
    public class RatesService : IRatesService, IDisposable
    {
        public readonly string DefaultCurrencyPairs = "EUR,GBP,JPY,ILS";

        const string alphavantage_secret_0 = "55Y1508W05UYQN3G";
        const string alphavantage_secret_1 = "3MEYVIGY6HV9QYMI";


        readonly HttpClient Client;
        public readonly ToUsdContext Context;// { get; private set; }
        readonly IHttpSpooler Spooler;
        private readonly ILogger<RatesService> Log;

        private static int status = 1;
        public static bool FisrtCall => Interlocked.Exchange(ref status, 0) > 0;
        public static ConcurrentDictionary<string, RateToUsd> Dict =
                new ConcurrentDictionary<string, RateToUsd>
                            (StringComparer.OrdinalIgnoreCase);
        public int MaxReadDelaySec { get; init; } = 3600;


        public RatesService(HttpClient _httpClient,
                            IHttpSpooler _spooler,
                            ToUsdContext context,
                            ILogger<RatesService> logger)//,
                                                         //   IConfiguration config)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/json,*/*");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "that2dollar");
            Spooler = _spooler;
            Client = _httpClient;
            Client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/html"));
            Client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/text"));

            Log = logger;
            Context = context;

        }

        public async Task TryInit()
        {
            if (!FisrtCall) return;
            try
            {
                var list = DefaultCurrencyPairs.Split(",").ToList();

                await Context.Rates.ForEachAsync(p =>
                {
                    list.Add(p.code);
                    Dict.TryAdd(p.code, p);
                });

            }
            catch (Exception ex)
            {
                Log.LogError(ex.StackTrace);
            }
        }

        public RateToUsd[] Rates => Dict.Values.ToArray();


        public async Task<RateToUsd> GetRatio(string code)
        {
            try
            {
                await TryInit();


                TestCode("code", ref code);

                RateToUsd rate = null;
                DateTime dt0 = DateTime.Now;
                double ms = 0;
                bool b = false;
                if (!(b = Dict.TryGetValue(code, out rate)) ||
                    (ms = (dt0 - rate.stored).TotalSeconds) > MaxReadDelaySec)
                {
                    rate = await RetrieveFromHttp(code);
                    if (rate != null)
                    {
                        Dict.AddOrUpdate(code, rate, (code, oldValue) => rate);

                        ToStore(rate);
                    }
                }
                return rate;
            }
            catch (Exception ex)
            {

                Log.LogError(ex.Message);
                return null;
            }
        }
        public async Task<FromTo> GetRatioForPair(string from, string to)
        {
            try
            {
                await TryInit();

                TestCode("from", ref from);
                TestCode("to", ref to);

                RateToUsd[] arr = await Task.WhenAll<RateToUsd>(
                       GetRatio(from), GetRatio(to));
                if (arr == null || arr.Length != 2 || arr[0] == null || arr[1] == null)
                {
                    return null;
                }
                var ret = new FromTo()
                {
                    from = arr[0],
                    to = arr[1],

                };

                return ret;
            }
            catch (Exception ex)
            {

                Log.LogError(ex.Message);
                return null;
            }
        }
        public async Task<bool> Remove(string code)
        {
            try
            {
                TestCode("code", ref code);
                RateToUsd rate;
                Dict.Remove(code, out rate);

                var b = Context.Rates.Any(p => p.code == code);
                if (b)
                {
                    Log.LogInformation($"DELETE RateToUsd {rate}");

                    var rate1 = Context.Rates.Remove(rate);
                    await Context.SaveChangesAsync();
                }
                return b;

            }
            catch (Exception ex)
            {

                Log.LogError(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        static object _lockStore = new Object();

        private bool ToStore(RateToUsd rate)
        {
            lock (_lockStore)
            {
                var b = Context.Rates.Any(p => p.code.ToUpper() == rate.code.ToUpper());
                if (!b)
                {
                    Context.Rates.Add(rate);
                    Log.LogInformation($"ADD RateToUsd {rate}");
                }
                else
                {
                    Context.Rates.Update(rate);
                    Log.LogInformation($"UPDATE RateToUsd {rate}");
                }
                Context.SaveChanges();

                return b;
            }
        }

        void TestCode(string name, ref string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(str);
            }
            str = str.ToUpper();
        }

        public string ConvertorName =>
              "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE";
  
        private async Task<RateToUsd> RetrieveFromHttp(string code)
        {

            try
            {
                string url = ConvertorName 
                           + "&from_currency=USD&to_currency=" + code;

                string url0 = url + "&apikey=" + alphavantage_secret_0;

                RatesServiceDecoder decoder = new RatesServiceDecoder(code, Log);

                RateToUsd body = await Spooler.GetHttp(Client, url0, decoder, MaxReadDelaySec) as RateToUsd;

                if (body == null )
                {
                    string url1 = url + "&apikey=" + alphavantage_secret_1;
      
                    body = await Spooler.GetHttp(Client, url1, decoder, MaxReadDelaySec) as RateToUsd;
                 }

                return body;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message + "\n" + ex.StackTrace);
                return null;
            }



        }
     

   
        public void Dispose()
        {
            //TBD ???   if(Context.opened)
            //  Context.Dispose();
        }


    }


}


/*
  const ratesExchangeAxios = {
"Realtime Currency Exchange Rate": {
 "1. From_Currency Code": "USD",
 "2. From_Currency Name": "United States Dollar",
 "3. To_Currency Code": "JPY",
 "4. To_Currency Name": "Japanese Yen",
 "5. Exchange Rate": "110.29100000",
 "6. Last Refreshed": "2021-04-06 08:16:01",
 "7. Time Zone": "UTC",
 "8. Bid Price": "110.29060000",
 "9. Ask Price": "110.29560000"
}
}

  */
