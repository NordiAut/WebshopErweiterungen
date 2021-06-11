using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webshop.ViewModel
{
    public class OrderLineProductViewModel
    {
        public int ID { get; set; }

        public Nullable<int> Order_Id { get; set; }

        [DisplayName("Amount")] 
        [RegularExpression("([1-9]|10)", ErrorMessage = "Count must be a natural number")]

        [Required]
        public Nullable<int> Amount { get; set; }

        [DisplayName("Price")]
        public Nullable<decimal> NetUnitPrice { get; set; }
        public Nullable<double> TaxRate { get; set; }

        [DisplayName("ProductNr")]
        public Nullable<int> Product_ID { get; set; }

        [DisplayName("Product")]
        public string Product_Name { get; set; }

        public string ImagePath { get; set; }

        [DisplayName("Manufacturer")]
        public string Manufacturer_Name { get; set; }

        [DisplayName("Total")]
        public Nullable<decimal> priceLine { get; set; }
        
      



    }
}