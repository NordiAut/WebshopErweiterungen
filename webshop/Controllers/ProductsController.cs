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

        public ActionResult Product(string searchString, string CategoryFilter = "", string ManufactorerFilter = "")
        {
            var vm = new FilterAndSearchViewModel();



            vm.CategoryListItem = new List<SelectListItem>();

            foreach (var category in db.Category)
            {
                vm.CategoryListItem.Add(
                        new SelectListItem { Value = category.Category_Name, Text = category.Category_Name }
                    );

            }

            vm.ManufacturerListItem = new List<SelectListItem>();

            foreach (var manufacturer in db.Manufacturer)
            {
                vm.ManufacturerListItem.Add(
                    new SelectListItem { Value = manufacturer.Manufacturer_Name, Text = manufacturer.Manufacturer_Name }
                );

            }

            var data = db.Product;
            var product = db.Product.Include(p => p.Category).Include(p => p.Manufacturer);

            string categoryName = CategoryFilter;
            string manufacturerName = ManufactorerFilter;



            // When input contains searchstring
            if (!String.IsNullOrEmpty(searchString))
            {
                product = db.Product.Where(p => p.Product_Name.Contains(searchString));

                // When category is selected
                if (!String.IsNullOrEmpty(CategoryFilter) && String.IsNullOrEmpty(ManufactorerFilter))
                {
                    product = db.Product.Where(p => p.Category.Category_Name == categoryName)
                        .Where(p => p.Product_Name.Contains(searchString));
                }

                // When manufacturer is selected
                if (!String.IsNullOrEmpty(ManufactorerFilter) && String.IsNullOrEmpty(CategoryFilter))
                {
                    product = db.Product.Where(p => p.Manufacturer.Manufacturer_Name == manufacturerName)
                        .Where(p => p.Product_Name.Contains(searchString));
                }

                // When manufacturer and category are selected
                if (!String.IsNullOrEmpty(ManufactorerFilter) && !String.IsNullOrEmpty(CategoryFilter))
                {
                    product = db.Product.Where(p => p.Manufacturer.Manufacturer_Name == manufacturerName)
                          .Where(p => p.Category.Category_Name == categoryName)
                          .Where(p => p.Product_Name.Contains(searchString));
                }
            }
            else
            {
                // When manufacturer and category are selected
                if (!String.IsNullOrEmpty(ManufactorerFilter) && !String.IsNullOrEmpty(CategoryFilter))
                {
                    product = db.Product.Where(p => p.Manufacturer.Manufacturer_Name == manufacturerName)
                        .Where(p => p.Category.Category_Name == categoryName);
                }
                if (!String.IsNullOrEmpty(CategoryFilter) && String.IsNullOrEmpty(ManufactorerFilter))
                {
                    product = db.Product.Where(p => p.Category.Category_Name == categoryName);
                }
                // When manufacturer is selected
                if (!String.IsNullOrEmpty(ManufactorerFilter) && String.IsNullOrEmpty(CategoryFilter))
                {
                    product = db.Product.Where(p => p.Manufacturer.Manufacturer_Name == manufacturerName);
                }
            }

            var productlist = product.ToList();

            Services.Helper.Shuffle(productlist);

            // vm.Data = list of product
            vm.Data = productlist;
            return View(vm);
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
                .OrderByDescending(o => o.Order_ID)
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

            var nettoTotal = total;
            var bruttoTotal = nettoTotal * 1.2M;
            var UST = bruttoTotal - nettoTotal;
            ViewBag.UST = Math.Round(UST, 2);
            //Total viewbag
            bruttoTotal = Math.Round(bruttoTotal, 2);
            ViewBag.Total = bruttoTotal;


            return View(showList);
        }

        [HttpPost]
        public ActionResult ShoppingCart(IList<webshop.ViewModel.OrderLineProductViewModel> orderlineList)
        {
            foreach (var orderLine in orderlineList)
            {
                var orderLineDB = db.OrderLine
                    .Include(o => o.Product)
                    .Include(o => o.OrderTable)
                    .Where(o => o.OrderLine_ID == orderLine.ID).FirstOrDefault();

                var product = db.Product.Where(p => p.Product_ID == orderLineDB.Product_ID).FirstOrDefault();

                // create new orderline 
                var newOrderLine = new OrderLine()
                {
                    Order_ID = orderLineDB.Order_ID,
                    Product_ID = orderLineDB.Product_ID,
                    Amount = orderLine.Amount,
                    NetUnitPrice = orderLineDB.NetUnitPrice,
                    TaxRate = product.Category.TaxRate
                };

                newOrderLine.Amount = orderLine.Amount;

                db.OrderLine.Add(newOrderLine);
                db.OrderLine.Remove(orderLineDB);
                
            }

            db.SaveChanges();

            return RedirectToAction("ShoppingCart");
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
                .OrderByDescending(o => o.Order_ID)
                .Where(x => x.Customer_ID == customerId).FirstOrDefault();

                var orderId = orderTable.Order_ID;
                // get list of orderlines
                var orderLine = db.OrderLine.Where(o => o.Order_ID == orderId).ToList();

                // get orderline with productID 
                var orderlineProduct = orderLine.Where(o => o.Product_ID == product.Product_ID).FirstOrDefault();

                // When not logged in, redirect to login
                

                // Check if list contains product
                if (orderlineProduct != null)
                {
                    if (orderlineProduct.Amount == 10)
                    {

                        return RedirectToAction("Product");
                    }
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
        public ActionResult Details(int id, int number)
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

                    if (orderlineProduct.Amount + number > 10)
                    {
                        newOrderLine.Amount = 10;
                    }
                    else
                    {
                        newOrderLine.Amount += number;
                    }


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

        // POST: Products/Remove/5
        
       
        public ActionResult Remove(int id)
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
            var orderlineProduct = orderLine.Where(o => o.OrderLine_ID == id).FirstOrDefault();

            db.OrderLine.Remove(orderlineProduct);
            db.SaveChanges();
            return RedirectToAction("ShoppingCart");
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
