using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace that2dollar.Services
{
    public interface ISpooledService<T> where T : class
    {
        public HttpClient Client { get;  }
        public HttpClient PrepareHttpClient(HttpClient _httpClient);
        public  SpoolItem<T> ToSpoolItem(T rate);
        public T[] AllData { get; }
        public  Task<T> GetItem(string code);
        public  Task<bool> RemoveItem(string code);
        //public Task<T> RetrieveFromHttp(string code);

        public Task TryInit();
        public T DecodeBody(string code, string jsonBody);

        public  string ConvertorUrl { get; }
    }
}
