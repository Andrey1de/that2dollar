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
        /// 3 Letters currency Code (GBP)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public  string Code { get; set; }
        /// <summary>
        /// Nsme of currency 
        /// </summary>
        public  string Name { get; set; }
        /// <summary>
        /// Rate to USA dollar
        /// </summary>
        public  double Rate { get; set; }
        /// <summary>
        /// Bid rate
        /// </summary>
        public  double Bid { get; set; }
        /// <summary>
        /// Ask rate
        /// </summary>
        public   double Ask { get; set; }
       /// <summary>
       /// Time when rate was stored
       /// </summary>
        public   DateTime Stored { get; set; }
        public   DateTime LastRefreshed { get; set; }

    }

    public class FromTo
    {
        public RateToUsd From { get; set; }
        public RateToUsd To { get; set; }

        public double Ratio =>
            (From != null & To != null && To.Rate != 0) ? From.Rate / To.Rate : 0;


    }

}


