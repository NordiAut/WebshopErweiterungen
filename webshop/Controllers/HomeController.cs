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
            return View();
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
                    newCustomer.PwHash = GetHash(user.Password);
                    newCustomer.Salt = GetSalt();

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
                   


                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Customer.Add(newCustomer);
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
                // Get Password hashed
                var f_password = GetHash(password);

                // Check DB for User
                var data = db.Customer.Where(s => s.Email.Equals(email) && s.PwHash.Equals(f_password)).ToList();

                
             //   byte[] hashedAndSaltedPwBytes = GetSaltedStringHash(password, salt);
                

                
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().Customer_ID;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.LogInError = "Login failed";
                    return RedirectToAction("Login");
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




        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

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