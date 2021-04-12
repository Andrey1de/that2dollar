using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace That2Dollar.Models
{
    public class CompanyMatch
    {
        public string Symbol { get; set; } //TESO",
        public string Name { get; set; } //Tesco Corporation USA",
        public string Type { get; set; } //Equity",
        public string Region { get; set; } //United States",
        public string MarketOpen { get; set; } //09:30",
        public string MarketClose { get; set; } //16:00",
        public string Timezone { get; set; } //UTC-04",
        public string Currency { get; set; } //USD",
        public double MatchScore { get; set; } //0.8889"


        }
    public class CompanyBestMatches
    {
        public string Keyword { get; set; }
        public Dictionary<string, CompanyMatch> BestMatches = new Dictionary<string, CompanyMatch>();



     
    }

}


/*
 * 
 https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=tesco&apikey=3MEYVIGY6HV9QYMI
{
    "bestMatches": [
        {
            "1. symbol": "TESO",
            "2. name": "Tesco Corporation USA",
            "3. type": "Equity",
            "4. region": "United States",
            "5. marketOpen": "09:30",
            "6. marketClose": "16:00",
            "7. timezone": "UTC-04",
            "8. currency": "USD",
            "9. matchScore": "0.8889"
        },
        {
            "1. symbol": "TSCO.LON",
            "2. name": "Tesco PLC",
            "3. type": "Equity",
            "4. region": "United Kingdom",
            "5. marketOpen": "08:00",
            "6. marketClose": "16:30",
            "7. timezone": "UTC+01",
            "8. currency": "GBP",
            "9. matchScore": "0.7273"
        },
        {
            "1. symbol": "TSCDF",
            "2. name": "Tesco plc",
            "3. type": "Equity",
            "4. region": "United States",
            "5. marketOpen": "09:30",
            "6. marketClose": "16:00",
            "7. timezone": "UTC-04",
            "8. currency": "USD",
            "9. matchScore": "0.7143"
        },
        {
            "1. symbol": "TSCDY",
            "2. name": "Tesco plc",
            "3. type": "Equity",
            "4. region": "United States",
            "5. marketOpen": "09:30",
            "6. marketClose": "16:00",
            "7. timezone": "UTC-04",
            "8. currency": "USD",
            "9. matchScore": "0.7143"
        },
    }
}
 */
