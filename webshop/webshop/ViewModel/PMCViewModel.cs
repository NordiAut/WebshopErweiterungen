using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webshop.ViewModel
{
    public class PMCViewModel
    {
        public int Product_ID { get; set; }

        public string Product_Name { get; set; }

        public string ImagePath { get; set; }

        public float NetUnitPrice { get; set; }

        public int Manufacturer_ID { get; set; }

        public string Manufacturer_Name { get; set; }
        public float TaxRate { get; set; }

    }
}