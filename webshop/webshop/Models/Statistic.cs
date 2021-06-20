using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webshop.Models
{
    public class Statistic
    {

        public decimal[] decimalValue { get; set; }
        public int[] Werte { get; set; }
        public string[] Labels { get; set; }
    }
}