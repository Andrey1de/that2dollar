using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace that2dollar.Services
{
    public abstract class AbstractRemoteService<T>
        : ISpooledService<T> where T : class
    {
        readonly HttpSpooler<T> Spooler;
        readonly protected ILogger Log;
        public HttpClient Client { get; }
        public virtual int MaxReadDelaySec { get; init; } = 3600;//TBD from  env

        readonly string ClassName;
        public AbstractRemoteService(
                            HttpClient _httpClient,
                            ILogger logger,
                            string _className)//,
                                              //   IConfiguration config)
        {
            ClassName = _className;
            Client = _httpClient;
            Log = logger;
            Spooler = new HttpSpooler<T>(Log, Client, ToSpoolItem);

        }

        public virtual void LogError(Exception ex)
        {
            Log.LogError(ClassName + "=>" + ex.Message + '\n' + ex.StackTrace);

        }
        public virtual HttpClient PrepareHttpClient(HttpClient _httpClient)
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

        public abstract T DecodeBody(string key, string jsonBody);
        public abstract string ConvertorUrl { get; }
        public T[] AllData => Spooler.AllData;
        protected virtual void AssertKey(string name, ref string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(str);
            }
            str = str.ToUpper();
        }

        public virtual async Task<T> GetItem(string key)
        {
            try
            {
                await TryInit();


                AssertKey("symbol", ref key);

                T item = Spooler.TryGetValue(key);
                if (item == null)
                {
                    item = await RetrieveFromHttp(key);
                    if (item != null)
                    {
                        Spooler.TryAddValue(key, item);

                    }
                }
                return item;
            }
            catch (Exception ex)
            {

                Log.LogError(ex.Message);
                return null;
            }
        }
        public virtual async Task<bool> RemoveItem(string key)
        {
            try
            {
                AssertKey("symbol", ref key);
                T item = Spooler.TryRemove(key);
                bool b = false;

                if (b = (item != null))
                {
                    Log.LogInformation($"DELETE {ClassName} {item}");

                }
                return b;

            }
            catch (Exception ex)
            {

                Log.LogError(ClassName + "=>" + ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }


        public virtual async Task<T> RetrieveFromHttp(string key)
        {

            try
            {
                string url = ConvertorUrl + key;

                string url0 = url + "&apikey=" + Secrets.ADVANTAGE_SECRET_0;

                T body = await Spooler.GetHttpWithSpool(key, url0, MaxReadDelaySec, DecodeBody);

                if (body == null)
                {
                    string url1 = url + "&apikey=" + Secrets.ADVANTAGE_SECRET_1;

                    body = await Spooler.GetHttpWithSpool(key, url1, MaxReadDelaySec, DecodeBody);
                }

                return body;
            }
            catch (Exception ex)
            {
                Log.LogError(ClassName + "=>" + ex.Message + "\n" + ex.StackTrace);
                return null;
            }

        }

        public abstract SpoolItem<T> ToSpoolItem(T item);
        //{
        //    return new SpoolItem<T>()
        //    {
        //        Key = rate.Symbol,
        //        Data = rate,
        //        ActualUntil = DateTime.Now.AddSeconds(MaxReadDelaySec)

        //    };
        //}


        static int status = 1;
        public static bool FisrtCall => Interlocked.Exchange(ref status, 0) > 0;
        public virtual async Task TryInit()
        {
            await Task.Delay(0);
            return;
            // if (!FisrtCall) return;
        }



        //static Type typeT = null;
        //static PropertyInfo[] propsT = [];
        //protected T AppendObject(T that, object o)
        //{
        //    if (typeT == null)
        //    {
        //        typeT = that.GetType();
        //        propsT = typeT.GetProperties();
        //    }
        //    foreach (PropertyInfo propertyInfo in propsT)
        //    {
        //        // do stuff here
        //    }
        //    return T;
        //}
        }
    }
