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
            List<Brand> brands = (from recb in db.ReColumnBrands
                                  where recb.ColumnID == id
                                  select recb.Brand).ToList();
            if (brands.Count == 0)
            {
                brands.AddRange((from recp in db.ReColumnProducts
                                 where recp.ColumnID == id
                                 select recp.Product.Brand).Distinct().Take(5 - brands.Count));
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

        public ActionResult SetProductBrand(int PID,int BID)
        {
            Product tmpProduct = db.Products.FirstOrDefault(r => r.PID == PID);
            if(tmpProduct != null)
            {
                tmpProduct.BID = BID;
                db.SaveChanges();
            }

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
            List<Brand> brands = (from recb in db.ReColumnBrands
                          where recb.ColumnID == id
                          select recb.Brand).ToList();
            //if (brands.Count == 0)
            //{
            //    brands.AddRange((from recp in db.ReColumnProducts
            //                     where recp.ColumnID == id
            //                     select recp.Product.Brand).Distinct().Take(5 - brands.Count));
            //}
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

        public ActionResult GetProductBrand(int id)
        {
            Product tmpProduct = db.Products.FirstOrDefault(r => r.PID == id);
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(new List<Brand>() { tmpProduct.Brand });
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