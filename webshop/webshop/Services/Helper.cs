using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace webshop.Services
{
    public class Helper
    {


        private static Random rng = new Random();

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void FinishOrder(int orderId, int customerId, decimal bruttoprice)
        {
            using (var db = new webshopEntities())
            {

                // get customer
                var customer = db.Customer.Where(x => x.Customer_ID == customerId).FirstOrDefault();
                // get order from customer
                var order = db.OrderTable.Where(x => x.Order_ID == orderId).FirstOrDefault();

                order.DateOrdered = DateTime.Now;
                order.PriceTotal = bruttoprice;

                //Add new order to customer ID
                var newOrder = new OrderTable();
                newOrder.Customer_ID = customerId;
                newOrder.Street = customer.Street;
                newOrder.City = customer.City;
                if (customer.Zip != null)
                {
                    newOrder.Zip = customer.Zip;
                }

                newOrder.FirstName = customer.FirstName;
                newOrder.LastName = customer.LastName;
                newOrder.LastName = customer.LastName;

                db.OrderTable.Add(newOrder);
                db.SaveChanges();

            }
        }

        public static void EmailActivation(string text, string email)
        {

            string from = "platz12@lap-itcc.net";
            using (MailMessage mail = new MailMessage(from, email))
            {
                mail.Subject = "Account activation";
                mail.Body = "Your Account activation link: " + text;

                //Attachment attPDF = new Attachment(new MemoryStream(byteArray), "Invoice");
                //mail.Attachments.Add(new Attachment(new MemoryStream(byteArray), "Invoice"));
                //mail.Attachments.Add(attPDF);

                mail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.your-server.de";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from, "platz12IT-SYST");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCredential;
                smtp.Port = 25;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);

            }

        }

        public static void SetCustomerActive(int customerId)
        {
            using (var db = new webshopEntities())
            {
                // get customer
                var customer = db.Customer.Where(x => x.Customer_ID == customerId).FirstOrDefault();

                customer.Active = true;

                db.Entry(customer);
                db.SaveChanges();
            }
        }

    }
}