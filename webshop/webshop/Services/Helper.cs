using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public static void UpdateVolumeDiscounts(string email)
        {
            using (var db = new webshopEntities())
            {
                
                
                // get customer
            var customer = db.Customer.Where(x => x.Email == email).FirstOrDefault();
            // get order from customer

            // get orderTable from customer
            var order = db.OrderTable
                .OrderByDescending(o => o.Order_ID)
                .Where(x => x.Customer_ID == customer.Customer_ID).FirstOrDefault();

                 var orderLineList = db.OrderLine.Where(o => o.Order_ID == order.Order_ID).ToList();


                 foreach (var orderLine in orderLineList)
                {
                    if (orderLine.Amount > 5)
                    {
                        orderLine.VolumeDiscount = true;
                        orderLine.NetLinePrice = orderLine.NetUnitPrice * (orderLine.Amount - 1);
                    }
                    else
                    {
                        orderLine.VolumeDiscount = false;
                        orderLine.NetLinePrice = orderLine.NetUnitPrice * orderLine.Amount;
                    }
                    db.Entry(orderLine).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }



    }
}