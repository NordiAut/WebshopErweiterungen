using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webshop.ViewModel
{
    public class OrderCustomerOrderLine
    {
        
        public int Order_Id { get; set; }

        public int Customer_Id { get; set; }
        public decimal PriceTotal { get; set; }
        public System.DateTime DateOrdered { get; set; }

        public string Email { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DeliveryStreet { get; set; }

        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Invalid Zip Code")]
        public string DeliveryZip { get; set; }
        [Required]
        public string DeliveryCity { get; set; }
        [Required]
        public string DeliveryFirstName { get; set; }
        [Required]
        public string DeliveryLastName { get; set; }

        public string Payment { get; set; }

        public List<OrderLineProductViewModel> OrderLineProductList { set; get; }
        public List<Product> RecommendedProducts { set; get; }

    }
}