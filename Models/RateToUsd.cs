using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace that2dollar.Models
{

    /// <summary>
    /// Represent information about currency
    /// </summary>
    public partial class RateToUsd
    {
        /// <summary>
        /// 3 Letters currency code (GBP)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public  string code { get; set; }
        /// <summary>
        /// Nsme of currency 
        /// </summary>
        public  string name { get; set; }
        /// <summary>
        /// Rate to USA dollar
        /// </summary>
        public  double rate { get; set; }
        /// <summary>
        /// Bid rate
        /// </summary>
        public  double bid { get; set; }
        /// <summary>
        /// Ask rate
        /// </summary>
        public   double ask { get; set; }
       /// <summary>
       /// Time when rate was stored
       /// </summary>
        public   DateTime stored { get; set; }
        public   DateTime lastRefreshed { get; set; }

    }

    public class FromTo
    {
        public RateToUsd from { get; set; }
        public RateToUsd to { get; set; }

        public double ratio =>
            (from != null & to != null && to.rate != 0) ? from.rate / to.rate : 0;


    }

}


