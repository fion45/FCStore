using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.FilterAttribute;
using FCStore.Models;

namespace FCStore.Controllers
{
    public class ManagerController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Manager/

        [MyAuthorizeAttribute]
        public ActionResult BannerManager()
        {
            List<BannerItem> result = db.BannerItems.ToList();
            return View(result);
        }

        [MyAuthorizeAttribute]
        public ActionResult ColumnManager()
        {
            return View();
        }

        [MyAuthorizeAttribute]
        public ActionResult CategoryManager()
        {
            return View();
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
