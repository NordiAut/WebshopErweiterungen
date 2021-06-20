using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using webshop.Models;

namespace webshop.Controllers
{
    public class HomeController : Controller
    {

        private webshopEntities db = new webshopEntities();

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

                    //Account activation 
                    var activationLink = $"http://localhost:{58869}/Home/AccountActivation?id={newCustomer.Customer_ID}";

                    Services.Helper.EmailActivation(activationLink, newCustomer.Email);


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

        public ActionResult AccountActivation(int id)
        {
            Services.Helper.SetCustomerActive(id);
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


                // If ccustomer is registered && active
                if (customer != null && customer.Active == true)
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
                    ViewBag.LogInError = "Email not registered or not activated";
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