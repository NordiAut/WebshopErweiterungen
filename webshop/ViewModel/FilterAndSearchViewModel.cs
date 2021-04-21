using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webshop.ViewModel
{
    public class FilterAndSearchViewModel
    {
        public IEnumerable<Product> Data { set; get; }
        public List<SelectListItem> CategoryListItem { set; get; }
        public List<SelectListItem> ManufacturerListItem { set; get; }
        public string CategoryFilter { set; get; }
        public string ManufactorerFilter { set; get; }
    }
}