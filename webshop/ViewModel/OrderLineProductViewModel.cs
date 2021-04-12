using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webshop.ViewModel
{
    public class OrderLineProductViewModel
    {
        public int ID { get; set; }

        public int Order_Id { get; set; }

        public int Amount { get; set; }


        public PMCViewModel PMCVM { get; set; }



    }
}