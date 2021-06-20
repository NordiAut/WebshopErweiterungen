using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using webshop.Models;
using webshop.ViewModel;

namespace webshop.Controllers
{
    public class HomeController : Controller
    {

        private webshopEntities db = new webshopEntities();

        [HttpGet]
        public ActionResult PasswordInvoice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordInvoice(string password)
        { 
            var email = Session["Email"].ToString();
            var customer = db.Customer.Where(s => s.Email.Equals(email)).FirstOrDefault();

            // Get Password hashed
            var hashPassword = GetSaltedStringHash(password, customer.Salt);


            var checkoutObject = new OrderCustomerOrderLine();

            var customerId = Convert.ToInt32(Session["idUser"]);
            // get order from customer
            var order = db.OrderTable
                .OrderByDescending(o => o.DateOrdered)
                .Where(x => x.Customer_ID == customerId).FirstOrDefault();
            
            // get list of orderlines from order from customer
            var orderId = order.Order_ID;
            var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();
            var cartlist = new List<OrderLineProductViewModel>();

            decimal total = 0.0m;

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

            // Netto and Brutto
            var nettoTotal = total;

            //Fill checkout object
            checkoutObject.Customer_Id = customer.Customer_ID;
            checkoutObject.Order_Id = order.Order_ID;
            checkoutObject.PriceTotal = total;
            checkoutObject.Email = customer.Email;
            checkoutObject.Street = order.Street;
            checkoutObject.Zip = order.Zip.ToString();
            checkoutObject.City = order.City;
            checkoutObject.FirstName = order.FirstName;
            checkoutObject.LastName = order.LastName;

            //fill delivery-data 
            checkoutObject.DeliveryStreet = order.DeliveryStreet;
            checkoutObject.DeliveryZip = order.DeliveryZip;
            checkoutObject.DeliveryCity = order.DeliveryCity;
            checkoutObject.DeliveryFirstName = order.DeliveryFirstName;
            checkoutObject.DeliveryLastName = order.DeliveryLastName;
            checkoutObject.Payment = order.Payment;


            // If password is valid 
            if (customer.PwHash.SequenceEqual(hashPassword))
            {
               
                return RedirectToAction("InvoicePdf", "Checkout", checkoutObject);
            }
            else
            {
                ViewBag.LogInError = "Password incorrect";
                return View();
            }


            
        }

        public ActionResult Index()
        {
            ViewBag.error = "Email already exists";
            return RedirectToAction("Landing");
        }

        public ActionResult Landing()
        {
            var product = db.Product.Take(3);
            return View(product.ToList());

        }

        // Register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // New Customer to create
                var newCustomer = new Customer();
                //Check if there is already customer with this email
                var check = db.Customer.FirstOrDefault(s => s.Email == user.Email);
                if (check == null)
                {
                    //Security
                    newCustomer.Salt = GetSalt();
                    newCustomer.PwHash = GetSaltedStringHash(user.Password, newCustomer.Salt);
                    

                    //Add new Customer
                    newCustomer.FirstName = user.FirstName;
                    newCustomer.LastName = user.LastName;
                    newCustomer.Email = user.Email;
                    newCustomer.Title = user.Title;
                    newCustomer.Street = user.Street;
                    if (user.Zip != null)
                    { 
                        newCustomer.Zip = Int32.Parse(user.Zip); 
                    }
                    newCustomer.City = user.City;

                    //Add new order to customer ID
                    var newOrder = new OrderTable();
                    newOrder.Customer_ID = newCustomer.Customer_ID;
                    newOrder.Street = newCustomer.Street;
                    newOrder.City= newCustomer.City;
                    if (newCustomer.Zip != null)
                    {
                        newOrder.Zip = newCustomer.Zip;
                    }
                    newOrder.FirstName = newCustomer.FirstName;
                    newOrder.LastName = newCustomer.LastName;


                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Customer.Add(newCustomer);
                    db.OrderTable.Add(newOrder);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }

            }

            return View();
        }

        //Login

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {

                // Check DB for User
                var customer = db.Customer.Where(s => s.Email.Equals(email)).FirstOrDefault();

                // When allowing multiple accounts with one email
                // var data = db.Customer.Where(s => s.Email.Equals(email) && s.PwHash.Equals(hashPassword)).ToList();


                // If ccustomer is registered
                if (customer != null)
                {
                    // Get Password hashed
                    var hashPassword = GetSaltedStringHash(password, customer.Salt);

                    // If password is valid 
                    if (customer.PwHash.SequenceEqual(hashPassword))
                    {
                        //add session
                        Session["FullName"] = customer.FirstName + " " + customer.LastName;
                        Session["Email"] = customer.Email;
                        Session["idUser"] = customer.Customer_ID;
                        
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.LogInError = "Password incorrect";
                        return View();
                    }

                }
                else
                {
                    ViewBag.LogInError = "Email not registered";
                    return View();
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }




        public ActionResult AGB()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Impressum()
        {
            ViewBag.Message = "Contact Data";

            return View();
        }

        private static byte[] GetHash(string str)
        {
            var hasher = new SHA256Managed();

            var strBytes = Encoding.UTF8.GetBytes(str);

            var hashedStrBytes = hasher.ComputeHash(strBytes);
            return hashedStrBytes;
        }

        private static byte[] GetSalt()
        {
            byte[] salt = new byte[256 / 8];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(salt);

            return salt;
        }
      

        private static byte[] GetSaltedStringHash(string password, byte[] salt)
        {
            //String in Bytes umwandeln
            byte[] strBytes = Encoding.UTF8.GetBytes(password);

            //Salt an String anhängen
            byte[] HashSalt = strBytes.Concat(salt).ToArray();


            var hasher = new SHA256Managed();

            return hasher.ComputeHash(HashSalt);
        }
    }
}