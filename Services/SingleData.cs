using that2dollar.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace that2dollar.Services
{
    public interface ISingleData
    {
        public ConcurrentDictionary<string, RateToUsd> Dict  { get; }
       
        public bool FisrtCall { get; }



    }

    public class SingleData : ISingleData
    {
        public readonly ConcurrentDictionary<string, RateToUsd> _Dict =
         new ConcurrentDictionary<string, RateToUsd>(StringComparer.OrdinalIgnoreCase);
        private int status = 1;
        public bool FisrtCall => Interlocked.Exchange(ref status, 0) > 0;
        public  ConcurrentDictionary<string, RateToUsd> Dict => _Dict;

        public SingleData()
        {

        }
    }
}
