using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using that2dollar.Utils;

namespace that2dollar.Services
{
    public interface ISpoolDecoder
    {
        public object DecodeBody(string data);
    }
    internal class SpoolItem
    {
        internal string Url;
        internal object Data;
        internal DateTime ActualUntil { get; set; }

    }

    public interface IHttpSpooler
    {
        public Task<object> GetHttp(HttpClient client ,string url, ISpoolDecoder decoder, int howLongToStoreSec);


    }
    public class HttpSpooler : IHttpSpooler
    {
        //public static readonly HttpSpooler Single = new HttpSpooler();
        //readonly HttpClient Client;
        readonly ILogger<HttpSpooler> Log;

        static ConcurrentDictionary<string, SpoolItem> Dict =
            new ConcurrentDictionary<string, SpoolItem>(StringComparer.OrdinalIgnoreCase);

        public HttpSpooler( ILogger<HttpSpooler> _logger)
        {
           // Client = _httpClient;
            Log = _logger;
         }

        public async Task<object> GetHttp(HttpClient client, string url, ISpoolDecoder decoder, int howLongToStoreSec)
        {
            object ret = null;

            SpoolItem item = Dict.AddOrUpdate(url,
                (_url) =>
                {
                    // Create New Instance for Spooler
                    return new SpoolItem()
                    {
                        Url = url,
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

            if (item.Data != null)
                return item.Data;

            using (var request = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri = new Uri(url)
                                                        })
            {
                using (var response = await client.SendAsync(request))
                {

                    response.EnsureSuccessStatusCode();
                    string jsonBody = await response.Content.ReadAsStringAsync();
                    item.Data = ret = decoder.DecodeBody(jsonBody);

                }
            }


            return item.Data;

        }


    }
}
