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
    public class CategoryController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public PartialViewResult _CategoryList()
        {
            //List<Category> result = db.Categorys.ToList();
            //result = (from category in result
            //          where category.ParCID == 1 && category.CID != 1
            //          select category).ToList();
            //return PartialView(result);


            return PartialView(db.Categorys);
        }

        public ActionResult GetTreeNodes()
        {
            if (Request.IsAjaxRequest())
            {
                //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(tmpVM);
                string jsonStr = PubFunction.BuildResult(db.Categorys);
                return Content(jsonStr);
            }
            else
            {
                return View(db.Categorys);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}