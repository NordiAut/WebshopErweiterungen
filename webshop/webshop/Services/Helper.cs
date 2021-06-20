using System;
using System.Collections.Generic;
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

        public static void UpdateProductStock(int orderId)
        {
            using (var db = new webshopEntities())
            {
                //Load
                var orderlines = db.OrderLine.Where(ol => ol.Order_ID == orderId).ToList();

                //Loop
                foreach (var orderLine in orderlines)
                {
                    //Load procuct
                    var dbProduct = db.Product.Where(p => p.Product_ID == orderLine.Product_ID).FirstOrDefault();

                    //Manipulate stock
                    if (dbProduct.Stock - orderLine.Amount < 10)
                    {
                        dbProduct.Stock = 20;
                    }
                    else
                    {
                        dbProduct.Stock -= Convert.ToInt32(orderLine.Amount);
                    }
                }
                db.SaveChanges();
            }
        }



    }
}