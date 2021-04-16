using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webshop;
using webshop.ViewModel;

namespace webshop.Controllers
{
    public class ProductsController : Controller
    {
        private webshopEntities db = new webshopEntities();

        // GET: Products
        public ActionResult Index()
        {
            var product = db.Product.Include(p => p.Category).Include(p => p.Manufacturer);
            return View(product.ToList());
        }

        public ActionResult Product(string searchString, string currentFilter, bool? filterM, bool? filterC)
        {
            // standard product list
            var product = db.Product.Include(p => p.Category).Include(p => p.Manufacturer);

            //Search box
            if (!String.IsNullOrEmpty(searchString))
            {
                product = db.Product
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Where(p => p.Product_Name.Contains(searchString)
                                    || p.Category.Category_Name.Contains(searchString)
                                    || p.Manufacturer.Manufacturer_Name.Contains(searchString));

                // When category is checked and Manufacturer unchecked
                if (filterM == false & filterC == true)
                {
                    product = db.Product
                        .Include(p => p.Category)
                        .Where(p => p.Category.Category_Name.Contains(searchString));
                }
                // When Manufacturer is checked and category unchecked
                if (filterM == true & filterC == false)
                {
                    product = db.Product
                        .Include(p => p.Category)
                        .Where(p => p.Manufacturer.Manufacturer_Name.Contains(searchString));
                }

            }


            return View(product.ToList());
        }

        


        public ActionResult ShoppingCart()
        {

            var customerId = Convert.ToInt32(Session["idUser"]);

            // get Customer
            var customer = db.Customer
                .Include(x => x.OrderTable)
                .Where(x => x.Customer_ID == customerId).FirstOrDefault();


            // get orderTable from customer
            var oderTable = db.OrderTable
                .Include(x => x.Customer)
                .Where(x => x.Customer_ID == customerId).FirstOrDefault();

            var orderId = oderTable.Order_ID;
            var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();
            var showList = new List<OrderLineProductViewModel>();

            decimal total = 0.0m;

            bool isEmpty = !orderLine.Any();
            if (isEmpty)
            {
                return RedirectToAction("EmptyCart");
            }

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
                showList.Add(temp_line);

               total += temp_line.priceLine ?? default;
            }

            

            total = Math.Round(total, 2);


            ViewBag.Total = total;


            return View(showList);
        }



        //function to add ONE item to the shopping cart
        public ActionResult Add(int? id)
        {
            // When not logged in, redirect to login
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Product.Where(p => p.Product_ID == id).FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            if (product != null)
            {

                var customerId = Convert.ToInt32(Session["idUser"]);

                // get Customer
                var customer = db.Customer
                    .Include(x => x.OrderTable)
                    .Where(x => x.Customer_ID == customerId).FirstOrDefault();


                // get orderTable from customer
                var orderTable = db.OrderTable
                    .Include(x => x.Customer)
                    .Where(x => x.Customer_ID == customerId).FirstOrDefault();

                var orderId = orderTable.Order_ID;
                // get list of orderlines
                var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();

                // get orderline with productID 
                var orderlineProduct = orderLine.Where(o => o.Product_ID == product.Product_ID).FirstOrDefault();

                // Check if list contains product
                if (orderlineProduct != null)
                {
                    // create new orderline
                    var newOrderLine = new OrderLine()
                    {
                        Order_ID = orderlineProduct.Order_ID,
                        Product_ID = orderlineProduct.Product_ID,
                        Amount = orderlineProduct.Amount,
                        NetUnitPrice = orderlineProduct.NetUnitPrice,
                        TaxRate = product.Category.TaxRate
                    };

                    //increment by 1 
                    newOrderLine.Amount++;

                    //change DB 
                    db.OrderLine.Add(newOrderLine);
                    db.OrderLine.Remove(orderlineProduct);
                    db.SaveChanges();

                } // If not add product to list
                else
                {
                    // create new orderline
                    var newOrderLine = new OrderLine()
                    {
                        Order_ID = orderTable.Order_ID,
                        Product_ID = product.Product_ID,
                        Amount = 1,
                        NetUnitPrice = product.NetUnitPrice,
                        TaxRate = product.Category.TaxRate
                    };

                    // change db
                    db.OrderLine.Add(newOrderLine);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Product");
        }

        
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //function to add multiple items to the shopping cart
        [HttpPost]
        public ActionResult Details(int? id, int? number)
        {
            // When not logged in, redirect to login
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Product.Where(p => p.Product_ID == id).FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            if (number == null)
            {
                number = 1;
            }

            if (product != null)
            {

                var customerId = Convert.ToInt32(Session["idUser"]);

                // get Customer
                var customer = db.Customer
                    .Include(x => x.OrderTable)
                    .Where(x => x.Customer_ID == customerId).FirstOrDefault();


                // get orderTable from customer
                var orderTable = db.OrderTable
                    .Include(x => x.Customer)
                    .Where(x => x.Customer_ID == customerId).FirstOrDefault();

                var orderId = orderTable.Order_ID;
                // get list of orderlines
                var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();

                // get orderline with productID 
                var orderlineProduct = orderLine.Where(o => o.Product_ID == product.Product_ID).FirstOrDefault();

                // Check if list contains product
                if (orderlineProduct != null)
                {
                    // create new orderline
                    var newOrderLine = new OrderLine()
                    {
                        Order_ID = orderlineProduct.Order_ID,
                        Product_ID = orderlineProduct.Product_ID,
                        Amount = orderlineProduct.Amount,
                        NetUnitPrice = orderlineProduct.NetUnitPrice,
                        TaxRate = product.Category.TaxRate
                    };

                    //add num
                    newOrderLine.Amount += number;

                    //change DB 
                    db.OrderLine.Add(newOrderLine);
                    db.OrderLine.Remove(orderlineProduct);
                    db.SaveChanges();

                } // If not add product to list
                else
                {
                    // create new orderline
                    var newOrderLine = new OrderLine()
                    {
                        Order_ID = orderTable.Order_ID,
                        Product_ID = product.Product_ID,
                        Amount = number,
                        NetUnitPrice = product.NetUnitPrice,
                        TaxRate = product.Category.TaxRate
                    };

                    // change db
                    db.OrderLine.Add(newOrderLine);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Product");
        }


      
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_ID = new SelectList(db.Category, "Category_ID", "Category_Name");
            ViewBag.Manufacturer_ID = new SelectList(db.Manufacturer, "Manufacturer_ID", "Manufacturer_Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_ID,Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice,ImagePath,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_ID = new SelectList(db.Category, "Category_ID", "Category_Name", product.Category_ID);
            ViewBag.Manufacturer_ID = new SelectList(db.Manufacturer, "Manufacturer_ID", "Manufacturer_Name", product.Manufacturer_ID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_ID = new SelectList(db.Category, "Category_ID", "Category_Name", product.Category_ID);
            ViewBag.Manufacturer_ID = new SelectList(db.Manufacturer, "Manufacturer_ID", "Manufacturer_Name", product.Manufacturer_ID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_ID,Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice,ImagePath,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Category, "Category_ID", "Category_Name", product.Category_ID);
            ViewBag.Manufacturer_ID = new SelectList(db.Manufacturer, "Manufacturer_ID", "Manufacturer_Name", product.Manufacturer_ID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
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

        public ActionResult EmptyCart()
        {
            
            return View();
        }
    }
}
