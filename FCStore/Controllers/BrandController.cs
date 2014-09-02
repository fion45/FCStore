using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class BrandController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public PartialViewResult _BrandList()
        {
            return PartialView(db.Brands.ToList());
        }

        public ActionResult GetSelectBrandInColum(int id)
        {
            Column tmpColum = db.Columns.FirstOrDefault(r => r.ColumnID == id);
            if (Request.IsAjaxRequest())
            {
                List<Brand> brands = tmpColum.Brands;
                if(brands.Count < 5)
                {
                    brands.AddRange((from p in tmpColum.Products
                                        select p.Brand).Distinct().Take(5 - brands.Count));
                }
                string jsonStr = PubFunction.BuildResult(brands);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}