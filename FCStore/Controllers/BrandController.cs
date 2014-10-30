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
                if(brands.Count == 0)
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

        public ActionResult SetSelectBrandsInColum(int id, List<ReColumnBrand> Par)
        {
            db.m_objcontext.ExecuteStoreCommand("DELETE ReColumnBrands WHERE ColumnID = " + id);
            foreach (ReColumnBrand item in Par)
            {
                db.ReColumnBrands.Add(item);
            }
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult("OK");
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetBrandsInColumn(int id)
        {
            Column tmpColumn = db.Columns.FirstOrDefault(r => r.ColumnID == id);
            List<Brand> brands = new List<Brand>();
            if(tmpColumn != null)
            {
                brands = (from product in tmpColumn.Products
                          select product.Brand).ToList();
            }
            if (Request.IsAjaxRequest())
            {
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