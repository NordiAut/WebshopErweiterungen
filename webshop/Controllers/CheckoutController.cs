using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webshop.ViewModel;

namespace webshop.Controllers
{
    public class CheckoutController : Controller
    {
        private webshopEntities db = new webshopEntities();

        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Checkout(int id)
        {
            var checkoutObject = new OrderCustomerOrderLine();

            var customerId = Convert.ToInt32(Session["idUser"]);

            // get customer
            var customer = db.Customer.Where(x => x.Customer_ID == customerId).FirstOrDefault();
            // get order from customer
            var order= db.OrderTable.Where(x => x.Customer_ID == customerId).FirstOrDefault();
            // get list of orderlines from order from customer
            var orderId = order.Order_ID;
            var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();
            var cartlist = new List<OrderLineProductViewModel>();

            decimal total = 0.0m;

            bool isEmpty = !orderLine.Any();
            if (isEmpty)
            {
                return RedirectToAction("EmptyCart","Products");
            }

            // fill cartlist
            foreach (var line in orderLine)
            {

                var product = db.Product.Where(x => x.Product_ID == line.Product_ID).FirstOrDefault();
                // Fill up line
                var temp_line = new OrderLineProductViewModel()
                {
                    ID = line.OrderLine_ID,
                    Order_Id = line.Order_ID,
                    Amount = line.Amount,
                    NetUnitPrice = line.NetUnitPrice,
                    TaxRate = line.TaxRate,
                    Product_ID = line.Product_ID,
                    Product_Name = product.Product_Name,
                    ImagePath = product.ImagePath,
                    Manufacturer_Name = product.Manufacturer.Manufacturer_Name,
                    priceLine = line.Amount * line.NetUnitPrice

                };
                cartlist.Add(temp_line);

                total += temp_line.priceLine ?? default;
            }

            //Total viewbag
            total = Math.Round(total, 2);
            ViewBag.Total = total;

            
            //Fill checkout object
            checkoutObject.Customer_Id = customer.Customer_ID;
            checkoutObject.Order_Id = order.Order_ID;
            checkoutObject.PriceTotal = total;
            checkoutObject.Email = customer.Email;
            checkoutObject.Street = order.Street;
            checkoutObject.Zip = order.Zip;
            checkoutObject.City = order.City;
            checkoutObject.FirstName = order.FirstName;
            checkoutObject.LastName = order.LastName;

            //fill delivery-data 
            checkoutObject.DeliveryStreet = order.Street;
            checkoutObject.DeliveryZip = order.Zip;
            checkoutObject.DeliveryCity = order.City;
            checkoutObject.DeliveryFirstName = order.FirstName;
            checkoutObject.DeliveryLastName = order.LastName;
            //add cartlist to checkoutObject
            checkoutObject.OrderLineProductList = cartlist;







            return View(checkoutObject);
        }
    }
}