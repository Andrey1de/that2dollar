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

namespace that2dollar.Services
{



    public interface IRatesService
    {
        string GetConvertorName();
        Task TryInit();
        RateToUsd[] Rates { get; }
        Task<RateToUsd> GetRatio(string code);
        Task<FromTo> GetRatioForPair(string from, string to);

        Task<bool> Remove(string code);
    }
    public class RatesService : IRatesService, IDisposable
    {
        public readonly string DefaultCurrencyPairs  = "EUR,GBP,JPY,ILS";

        const string alphavantage_secret_0 = "55Y1508W05UYQN3G";
        const string alphavantage_secret_1 = "3MEYVIGY6HV9QYMI";

   
        readonly HttpClient Client ;


        private readonly ILogger<RatesService> Log;
        private static int status = 1;
        public static bool FisrtCall => Interlocked.Exchange(ref status, 0) > 0;
        public static ConcurrentDictionary<string, RateToUsd> Dict =
                new ConcurrentDictionary<string, RateToUsd>();
        public int MaxReadDelayMsec { get; init; } = 3600 * 1000;
        public readonly ToUsdContext Context;// { get; private set; }

        // ISingleData Single;

        public RatesService( HttpClient _httpClient,
                            ToUsdContext context,
                            ILogger<RatesService> logger)//,
                         //   IConfiguration config)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/json,*/*");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "that2dollar");

            Client = _httpClient;
            Log = logger;
            Context = context;
   
        }
      
        /// <summary>
        /// Async read of 
        /// </summary>
        public async Task  TryInit()
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

     /*           HashSet<string> unique_code = new HashSet<string>(list.ToArray());

                // Don't forget
                foreach (var code in unique_code)
                {
                    var p = await GetRatio(code);
                }
*/            
             
   
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
                    (ms = (dt0 - rate.stored).TotalMilliseconds)> MaxReadDelayMsec)
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
                if(arr == null || arr.Length != 2 || arr[0] == null || arr[1] == null)
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

        public string GetConvertorName()
        {
            return "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE";
        }

        private async Task<RateToUsd> RetrieveFromHttp(string code)
        {
           
            try
            {
                string url = GetConvertorName() + "&from_currency=USD&to_currency=" + code;

                string url0 = url + "&apikey=" + alphavantage_secret_0;
                string jsonBody = await ReadAsStringAsync(url0);
                if (!jsonBody.Contains("1. From_Currency Code"))
                {
                    string url1 = url + "&apikey=" + alphavantage_secret_1;

                    jsonBody = await ReadAsStringAsync(url1);
                }


                string[] pars1 = new string[] {
                    "\"4. To_Currency Name\":",
                    "\"5. Exchange Rate\":",
                    "\"6. Last Refreshed\":",
                    "\"8. Bid Price\":" ,
                    "\"9. Ask Price\":"};

                string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);

                if(arrr == null || arrr.Length < 6)
                {
                    Log.LogWarning($"Error jsonBody= [{arrr}]");
                    return null;
                }
   
                RateToUsd ret = new RateToUsd();
                ret.code = code.ToUpper();

                if (string.IsNullOrWhiteSpace(ret.code))
                {
                    throw new KeyNotFoundException(code);
                }
                
                ret.name = f1(arrr[1]);
                ret.rate = Double.Parse(f1(arrr[2]));
                ret.lastRefreshed = DateTime.Parse(f1(arrr[3]));
                ret.ask = Double.Parse(f1(arrr[4]));
                ret.bid = Double.Parse(f1(arrr[5]));
                ret.stored = DateTime.Now;


                return ret;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message + "\n" + ex.StackTrace);
                return null;
            }



        }
        Func<string, string> f1 = (str) =>
       str.Trim().Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];

        public async Task<string> ReadAsStringAsync(string url)
        {
            try
            {
                HttpResponseMessage resp = await Client.GetAsync(url);
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    return await resp.Content.ReadAsStringAsync();
                }
                else
                {
                    Log.LogWarning($"get : \"{url}\" returns status code {resp.StatusCode}");
                }
            }
            catch (Exception ex)
            {

                Log.LogError($"get : \"{url}\" \n exception {ex.Message}");
            }
            return "";

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
