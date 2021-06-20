using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webshop.Services
{
    public class Producthelper
    {

        public static List<Product> GetRecommendedProducts(int orderId)
        {

            Random rng = new Random();
            using (var db = new webshopEntities())
            {

                List<int> distinctCategoryIds = db.OrderLine
                    .Where(ol => ol.Order_ID == orderId)
                    .Select(ol => ol.Product.Category_ID)
                    .Distinct()
                    .ToList();

                var numCategories = distinctCategoryIds.Count();

                List<Product> productlist = new List<Product>();

                var counter = 0;


                do
                {
                    foreach (var categoryId in distinctCategoryIds)
                    {
                        //List of all products from categoryId
                        var allProductIdsInCategory = db.Product.Where(p => p.Category_ID == categoryId)
                            .Select(p => p.Product_ID).ToList();


                        // list of all ordered products from one category
                        var orderedProductIdsInCategory = db.OrderLine.Where(ol => ol.Order_ID == orderId)
                            .Select(ol => ol.Product_ID).ToList();


                        //Remove all products that got ordered last.
                        var recommendableIdsInCategory =
                            allProductIdsInCategory.Except(orderedProductIdsInCategory).ToList();


                        Product product1;
                        // get unique product
                        // While productlist contains product
                        // get random item
                        do
                        {
                            var numProducts = recommendableIdsInCategory.Count;
                            var randomIndex = rng.Next(1, numProducts);
                            var productId1 = recommendableIdsInCategory[randomIndex];
                            product1 = db.Product.Where(p => p.Product_ID == productId1).FirstOrDefault();
                        } while (productlist.Contains(product1));



                        // add product list
                        productlist.Add(product1);

                        // add counter
                        counter++;

                        if (counter >= 3)
                        {
                            return productlist;
                        }

                    }

                } while (counter <= 2);

                return productlist;
            }
        }
    }
}