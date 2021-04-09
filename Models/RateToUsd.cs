using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace that2dollar.Models
{

    public partial class RateToUsd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public  string code { get; set; }
        public  string name { get; set; }
        public  double rate { get; set; }
        public  double bid { get; set; }
        public   double ask { get; set; }
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


