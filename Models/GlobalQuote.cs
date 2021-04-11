using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace that2dollar.Models
{
    public class GlobalQuote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Symbol { get; set; } = "";// "IBM";//01
        public  double  Open { get; set; } = 0;// "133.7600";//02
        public  double  High { get; set; } = 0;// "133.9300";//03
        public  double  Low { get; set; } = 0;// "132.2700";//04
        public  double  Price { get; set; } = 0;//  integer "133.2300";//05
        public  double  Volume { get; set; } = 0;// integer "4074161";//06
        public string   LatestTradingDay { get; set; } = "";// "2021-04-01";//07
        public  double  PreviousClose { get; set; } = 0;// "133.2600";//08
        public  double  Change { get; set; } = 0;// "-0.0300";//09
        public  string  ChangePercent { get; set; } = "0%";// "-0.0225%";//10
        public DateTime Updated =  DateTime.Now;


    }
}
/*
  https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=IBM&apikey=55Y1508W05UYQN3G

{
    "Global Quote": {
        "01. symbol": "IBM",
        "02. open": "134.8700",
        "03. high": "135.7400",
        "04. low": "134.7100",
        "05. price": "135.7300",
        "06. volume": "3023916",
        "07. latest trading day": "2021-04-09",
        "08. previous close": "135.1200",
        "09. change": "0.6100",
        "10. change percent": "0.4515%"
    }
}
 */
//symbol: string = '';// "IBM";//01
//open: number { get; set; } = 0;// "133.7600";//02
//high: number { get; set; } = 0;// "133.9300";//03
//low: number { get; set; } = 0;// "132.2700";//04
//price: number { get; set; } = 0;//  integer "133.2300";//05
//volume: number { get; set; } = 0;// integer "4074161";//06
//latestTradingDay: string = '';// "2021-04-01";//07
//previousClose: number { get; set; } = 0;// "133.2600";//08
//change: number { get; set; } = 0;// "-0.0300";//09
//changePercent: number { get; set; } = 0;// "-0.0225%";//10
//updated: Date = new Date();

