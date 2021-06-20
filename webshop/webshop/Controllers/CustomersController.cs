using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using webshop;
using webshop.Models;

namespace webshop.Controllers
{
    public class CustomersController : Controller
    {
       


        private webshopEntities db = new webshopEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customer.ToList());
        }

        [HttpGet]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(Customer model)
        {
            {
                if (ModelState.IsValid)
                {
                    // get customer
                    var customer = db.Customer.Where(x => x.Email == model.Email).FirstOrDefault();

                    // If the user is found AND Email is confirmed
                    if (customer != null )
                    {
                        db.Entry(customer);
                        // Generate the reset password token
                        string token = Services.Helper.Token();

                        // save token in db

                        customer.Token = token;
                        db.SaveChanges();

                        // Build the password reset link
                        var passwordResetLink = $"http://localhost:{58869}/Customers/ResetPassword?email={model.Email}&token={token}";


                        Services.Helper.EmailResetPassword(passwordResetLink, model.Email);
                   

                        // Send the user to Forgot Password Confirmation view
                        return View("ForgotPasswordConfirmation");
                    }

                    // To avoid account enumeration and brute force attacks, don't
                    // reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                return View(model);
            }
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(User user)
        {
           
                // Find the user by email
                var customer = db.Customer.Where(x => x.Email == user.Email).FirstOrDefault();

                if (customer != null)
                {
                    // if token is valid
                    if (user.Token == customer.Token)
                    {
                        // reset the user password and invalidate token
                        db.Entry(customer);
                        customer.PwHash = Services.Helper.GetSaltedStringHash(user.Password, customer.Salt);
                        customer.Token = null;
                         db.SaveChanges();

                        return View("ResetPasswordConfirmation");

                    }
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View();
            
            
        }



        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Customer_ID,Title,FirstName,LastName,Email,Street,Zip,City,PwHash,Salt")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customer.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Customer_ID,Title,FirstName,LastName,Email,Street,Zip,City,PwHash,Salt")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customer.Find(id);
            db.Customer.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
     
    }
}
