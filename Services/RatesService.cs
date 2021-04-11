using that2dollar.Data;
using that2dollar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;

namespace that2dollar.Services
{



    public interface IRatesService : ISpooledService<RateToUsd>
    {
        public  Task<FromTo> GetRatioForPair(string from, string to);
        //public HttpClient Client { get;  }

        //public string ConvertorUrl { get; }
        //public Task TryInit();
        //public RateToUsd[] AllData { get; }
        //public Task<RateToUsd> GetItem(string code);
        //public Task<FromTo> GetRatioForPair(string from, string to);
        //public HttpClient PrepareHttpClient(HttpClient _httpClient);

        //public Task<bool> RemoveItem(string code);
    }
    public class RatesService : IRatesService, IDisposable
    {
        const bool TO_STORE = false;//TBD from env
        public readonly string DefaultCurrencyPairs = "EUR,GBP,JPY,ILS";

        const string alphavantage_secret_0 = "55Y1508W05UYQN3G";
        const string alphavantage_secret_1 = "3MEYVIGY6HV9QYMI";

        readonly HttpSpooler<RateToUsd> Spooler;

        public HttpClient Client { get; private set; }
        public readonly ToUsdContext Context;// { get; private set; }
        private readonly ILogger<RatesService> Log;


    
        public int MaxReadDelaySec { get; init; } = 3600;//TBD from  env

      

        public RatesService(ToUsdContext context,HttpClient _httpClient,
                            ILogger<RatesService> logger)//,
                                                         //   IConfiguration config)
        {
            Client = _httpClient;
            Log = logger;
            Context = context;
            Spooler = new HttpSpooler<RateToUsd>(Log, Client, ToSpoolItem);
         
        }

        public  RateToUsd DecodeBody(string code, string jsonBody)
        {
            if (!jsonBody.Contains("1. From_Currency Code"))
            {
                Log.LogWarning($"DecodeBody :Error in format jsonBody= {{{jsonBody}}}");
                return null;
            }
            string[] pars1 = new string[] {
                    "\"3. To_Currency Code\":",
                    "\"4. To_Currency Name\":",
                    "\"5. Exchange Rate\":",
                    "\"6. Last Refreshed\":",
                    "\"8. Bid Price\":" ,
                    "\"9. Ask Price\":"};

            string[] arrr = jsonBody.Split(pars1, StringSplitOptions.RemoveEmptyEntries);

            var code1 = f1(arrr[1]).ToUpper();
            if (arrr == null || arrr.Length < 7 || code1 != code)
            {
                Log.LogWarning($"DecodeBody : Error of Parsing= [{arrr}]");
                return null;
            }

            RateToUsd ret = new RateToUsd();
            ret.code = code;


            if (string.IsNullOrWhiteSpace(ret.code)
                || ret.code != code)
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

        public HttpClient PrepareHttpClient(HttpClient _httpClient)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/json,*/*");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "that2dollar");
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/html"));
            _httpClient.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/text"));
            return _httpClient;
        }

        public SpoolItem<RateToUsd> ToSpoolItem(RateToUsd rate)
        {

            return new SpoolItem<RateToUsd>()
            {
                Key = rate.code,
                Data = rate,
                ActualUntil = rate.lastRefreshed.AddSeconds(MaxReadDelaySec)

            };
        }
        private static int status = 1;
        public static bool FisrtCall => Interlocked.Exchange(ref status, 0) > 0;
        public async Task TryInit()
        {
            if (!FisrtCall) return;
            try
            {
                var list = DefaultCurrencyPairs.Split(",").ToList();

                await Context.Rates.ForEachAsync(rate =>
                {
                    Spooler.TryAddValue(rate.code, rate);
                });

            }
            catch (Exception ex)
            {
                Log.LogError(ex.StackTrace);
            }
        }

        public RateToUsd[] AllData => Spooler.AllData;


        public async Task<RateToUsd> GetItem(string code)
        {
            try
            {
                await TryInit();


                AssertCode("code", ref code);

                RateToUsd rate= Spooler.TryGetValue(code);
                if (rate == null)
                {
                    rate = await RetrieveFromHttp(code);
                    if (rate != null)
                    {
                         Spooler.TryAddValue(code, rate);

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

                AssertCode("from", ref from);
                AssertCode("to", ref to);

                RateToUsd[] arr = await Task.WhenAll<RateToUsd>(
                       GetItem(from), GetItem(to));
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
        public async Task<bool> RemoveItem(string code)
        {
            try
            {
                AssertCode("code", ref code);
                RateToUsd rate =  Spooler.TryRemove(code);
                bool b = false;

                if (b = (rate != null))
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
            if(!TO_STORE)
            {
                return true;
            }
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

        void AssertCode(string name, ref string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(str);
            }
            str = str.ToUpper();
        }

        public string ConvertorUrl => //TBD from env
              "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=";

        public async Task<RateToUsd> RetrieveFromHttp(string code)
        {

            try
            {
                string url = ConvertorUrl + code;

                string url0 = url + "&apikey=" + alphavantage_secret_0;

               

                RateToUsd body = await Spooler.GetHttpWithSpool(code, url0,  MaxReadDelaySec, DecodeBody);

                if (body == null)
                {
                    string url1 = url + "&apikey=" + alphavantage_secret_1;

                    body = await Spooler.GetHttpWithSpool(code, url1, MaxReadDelaySec,DecodeBody);
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
