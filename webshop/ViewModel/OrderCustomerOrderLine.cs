using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webshop.ViewModel
{
    public class OrderCustomerOrderLine
    {
        
        public Nullable<int> Order_Id { get; set; }

        public Nullable<int> Customer_Id { get; set; }
        public Nullable<decimal> PriceTotal { get; set; }
        public Nullable<System.DateTime> DateOrdered { get; set; }

        public string Email { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DeliveryStreet { get; set; }
        public string DeliveryZip { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryFirstName { get; set; }
        public string DeliveryLastName { get; set; }

        public List<OrderLineProductViewModel> OrderLineProductList { set; get; }

    }
}