using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using Rotativa.Options;
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

        public ActionResult PaymentOptions(int orderId, OrderCustomerOrderLine orderobject)
        {
            
            var orderObject = new OrderCustomerOrderLine();
            orderObject = OrderCustomerOrderLineList.Where(x => x.Order_Id == orderId).FirstOrDefault();
            if (orderobject.Payment == null)
            {
                return RedirectToAction("Invoice", orderObject);
            }
            else if (orderobject.Payment == "invoice")
            {
                return RedirectToAction("Invoice", orderObject);
            }
            return RedirectToAction("Checkout");
        }

        public ActionResult Invoice(OrderCustomerOrderLine orderObject)
        {
            var invoice = new Rotativa.ActionAsPdf("InvoicePDF", orderObject);
            // create invoice PDF
            var invoicePDF = new Rotativa.ActionAsPdf("InvoicePDF", orderObject)
            {
                FileName = "TestView.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = { Left = 1, Right = 1 }
            };
            byte[] byteArray = invoicePDF.BuildPdf(ControllerContext);

            var fileStream = new FileStream("B:/Applikationsentwickler/invoice.pdf", FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            //TODO Email
            string from = "oliver.rotter2@gmail.com"; 
            using (MailMessage mail = new MailMessage(from, "itn132163@qualifizierung.at"))
            {
                mail.Subject = "Invoice";
                mail.Body = "Thanks for ordering!";

                //Attachment attPDF = new Attachment(new MemoryStream(byteArray), "Invoice");
                //mail.Attachments.Add(new Attachment(new MemoryStream(byteArray), "Invoice"));
                //mail.Attachments.Add(attPDF);

                mail.Attachments.Add(new Attachment(@"B:/Applikationsentwickler/invoice.pdf"));
                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from, "Nordland12!");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
                
            }

            //return invoice;
            return View(orderObject);
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