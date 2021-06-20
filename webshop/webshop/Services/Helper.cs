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



        public static decimal CouponActivation(decimal price, string couponName, int CustomerId)
        {

            using (var db = new webshopEntities())
            {
                var coupon = db.Coupon.Where(C => C.Coupon_Name == couponName).FirstOrDefault();

                var discountPercentage = coupon.Discount;

                var moneyToDiscount = price / 100 * discountPercentage;

                var discountedPrice = price - moneyToDiscount;

                CouponCustomer usedCoupon = new CouponCustomer();

                usedCoupon.Coupon_ID = coupon.Coupon_ID;
                usedCoupon.Customer_ID = CustomerId;


                return discountedPrice;

            }
        }

        public static bool CouponCheck(string couponName, int CustomerId)
        {

            using (var db = new webshopEntities())
            {
                CouponCustomer couponCustomer = new CouponCustomer();
                
                var coupon = db.Coupon.Where(C => C.Coupon_Name == couponName).FirstOrDefault();
                if (coupon != null)
                {
                    couponCustomer = db.CouponCustomer
                        .Where(c => (c.Coupon_ID == coupon.Coupon_ID) && (c.Customer_ID == CustomerId)).FirstOrDefault();
                }

                bool check = false;

                if (couponCustomer == null)
                {
                    check = true;
                }

                return check;

            }
        }

        public static void CouponSave(string couponName, int CustomerId)
        {

            using (var db = new webshopEntities())
            {
                var coupon = db.Coupon.Where(C => C.Coupon_Name == couponName).FirstOrDefault();

                CouponCustomer couponCustomer = new CouponCustomer();

                couponCustomer.Customer_ID = CustomerId;
                couponCustomer.Coupon_ID = coupon.Coupon_ID;

                db.CouponCustomer.Add(couponCustomer);
                db.SaveChanges();

            }
        }
    }
}