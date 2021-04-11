using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using that2dollar.Utils;

namespace that2dollar.Services
{
    public class SpoolItem<TItem> where TItem : class
    {
        public string Key;
        public TItem Data = default;
        public DateTime ActualUntil { get; set; }
    }

    public abstract class SpoolDecoder<T>
    {
        public abstract T Decode(string key,string jsonBody);

    }


    public class HttpSpooler<T> where T : class
    {
        //public static readonly HttpSpooler Single = new HttpSpooler();
        //readonly HttpClient Client;
        readonly ILogger Log;
        readonly HttpClient Client;

        static Lazy<ConcurrentDictionary<string, SpoolItem<T>>> DictLazy =
            new Lazy<ConcurrentDictionary<string, SpoolItem<T>>>(
                new ConcurrentDictionary<string, SpoolItem<T>>(StringComparer.OrdinalIgnoreCase));
       
        public ConcurrentDictionary<string, SpoolItem<T>> Dict => DictLazy.Value;

             Func<T, SpoolItem<T>> ToSpoolItem;

        public HttpSpooler(ILogger _logger, HttpClient _client,
             Func<T, SpoolItem<T>> _toSpoolItemSpoler)
        {
            // Client = _httpClient;
            Log = _logger;
            Client = _client;
            ToSpoolItem = _toSpoolItemSpoler;
        }

        public T[] AllData => Dict.Values.Select(p => p.Data).ToArray();
        public T TryAddValue(string key,T p)
        {
            SpoolItem<T> item =  Dict.AddOrUpdate(key, item => ToSpoolItem(p), (item, old) => ToSpoolItem(p));
             return item.Data;

        }
        public T TryGetValue(string key)
        {
            SpoolItem<T> item = null;
            if (Dict.TryGetValue(key, out item))
            {
                if (item.ActualUntil >= DateTime.Now)
                {
                    Dict.TryRemove(key,out item);
                    item = null;
                }
            }

            return (item != null) ? item.Data : null;

        }
        public T TryRemove(string key)
        {
            
            SpoolItem<T> spool;
            return (Dict.TryRemove(key, out spool)) ?
                spool.Data : null;
       
        }

      
        public async Task<T> GetHttpWithSpool(string key, string url,  int howLongToStoreSec, 
                Func<string,string,T> decoder)
        {
            T data = null;
            try
            {
                SpoolItem<T> item = TrySpool(key, url, howLongToStoreSec);
                if (item.Data != null)
                {
                    return item.Data;

                }
                string jsonBody = await Client.GetHttpStringAsync(url);



                item.Data =  decoder(key,jsonBody);

                return item.Data;
            }
            catch (Exception ex)
            {

                Log.LogError(ex.StackTrace);
                return null; 
            }
            return null;

        }
 
        private SpoolItem<T> TrySpool(string key , string url, int howLongToStoreSec )
        {
            SpoolItem<T> item = DictLazy.Value.AddOrUpdate(key,
                (key) =>
                {
                    // Create New Instance for Spooler
                    return new SpoolItem<T>()
                    {
                        Key = key,
                        ActualUntil = DateTime.Now.AddSeconds(howLongToStoreSec),
                        Data = null
                    };
                },
                (_url, _item) =>
                {
                    // Test of Existing Instance for Time Existance
                    if (_item.ActualUntil < DateTime.Now)
                    {
                        _item.Data = null;
                        Log.LogInformation($"Old tem {url} removed");
                    }
                    return _item;
                }
                );
           return item;
        }

    }
}
