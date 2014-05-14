using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;

namespace FCStore.Controllers
{
    public class AreaController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        //
        // GET: /Area/

        public ActionResult Index()
        {
            return View(db.Areas.ToList());
        }

        //
        // GET: /Area/Details/5

        public ActionResult Details(int id = 0)
        {
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        //
        // GET: /Area/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Area/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Area area)
        {
            if (ModelState.IsValid)
            {
                db.Areas.Add(area);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(area);
        }

        //
        // GET: /Area/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        //
        // POST: /Area/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Area area)
        {
            if (ModelState.IsValid)
            {
                db.Entry(area).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(area);
        }

        //
        // GET: /Area/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        //
        // POST: /Area/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Area area = db.Areas.Find(id);
            db.Areas.Remove(area);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}