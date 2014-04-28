using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;

namespace FCStore.Controllers
{
    public class HomeController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("_ColumnList", "Column");
        }

    }
}
