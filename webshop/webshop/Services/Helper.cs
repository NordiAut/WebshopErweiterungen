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

        public static bool IsCardNumberValid(string cardNumber) //Luhn
        {
            cardNumber = cardNumber.Replace(" ", String.Empty);
            int totalSum = 0;
            bool odd = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cardNumber[i].ToString());
                if (odd)
                {
                    var actualNum = digit * 2;

                    if (actualNum > 9) actualNum = actualNum - 9;

                    totalSum += actualNum;
                }
                else
                {
                    totalSum += digit;
                }
                odd = !odd;
            }
            return totalSum % 10 == 0;
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


    }
}