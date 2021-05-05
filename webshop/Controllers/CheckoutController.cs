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
        public static List<OrderCustomerOrderLine> OrderCustomerOrderLineList = new List<OrderCustomerOrderLine>();

        
        public ActionResult Index()
        {
            return View();
        }

        // GET: Checkout
        [HttpGet]
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

            OrderCustomerOrderLineList.Remove(OrderCustomerOrderLineList.Where(x => x.Order_Id == checkoutObject.Order_Id).FirstOrDefault());
            OrderCustomerOrderLineList.Add(checkoutObject);





            return View(checkoutObject);
        }

        [HttpPost]
        public ActionResult Checkout(OrderCustomerOrderLine orderobject)
        {
            var tempOrder = OrderCustomerOrderLineList.Where(x => x.Order_Id == orderobject.Order_Id).FirstOrDefault();

            tempOrder.DeliveryFirstName = orderobject.DeliveryFirstName;
            tempOrder.DeliveryLastName = orderobject.DeliveryLastName;
            tempOrder.DeliveryStreet = orderobject.DeliveryStreet;
            tempOrder.DeliveryZip = orderobject.DeliveryZip;
            tempOrder.DeliveryCity = orderobject.DeliveryCity;



            return View(tempOrder);
        }

        public ActionResult PaymentOptions(int orderId, string payment)
        {
            
            var orderObject = new OrderCustomerOrderLine();
            orderObject = OrderCustomerOrderLineList.Where(x => x.Order_Id == orderId).FirstOrDefault();
            if (payment == null)
            {
                return RedirectToAction("Invoice", orderObject);
            }
            else if (payment == "invoice")
            {
                return RedirectToAction("Invoice", orderObject);
            }
            return RedirectToAction("Checkout");
        }

        public ActionResult Invoice(OrderCustomerOrderLine orderObject)
        {
            
            // create invoice PDF
            var invoicePDF = new Rotativa.ActionAsPdf("InvoicePDF", orderObject);

            //TODO Email
            

            return invoicePDF;
            //return View(orderObject);
        }

        public ActionResult InvoicePDF(OrderCustomerOrderLine orderObject)
        {
            // fill cartlist
            var orderLine = db.OrderLine.Where(o => o.Order_ID == orderObject.Order_Id).ToList();
            var cartlist = new List<OrderLineProductViewModel>();

            decimal total = 0.0m;
            
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

            //add cartlist to checkoutObject
            orderObject.OrderLineProductList = cartlist;

            return View(orderObject);
        }


    }
}