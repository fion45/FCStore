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
    public class ColumnController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public PartialViewResult _BigItem(Column col)
        {
            return PartialView(col.Products[0]);
        }

        public PartialViewResult _SmallItem(Column col)
        {
            List<Product> tmpList = new List<Product>();
            int tmpL = Math.Min(col.Products.Count,3);
            for (int i = 1; i < tmpL;i++ )
            {
                tmpList.Add(col.Products[i]);
            }
            return PartialView(tmpList);
        }

        public ActionResult _ColumnList()
        {
            return View(db.Columns.ToList());
        }

        //
        // GET: /Column/

        public ActionResult Index()
        {
            return View(db.Columns.ToList());
        }

        //
        // GET: /Column/Details/5

        public ActionResult Details(int id = 0)
        {
            Column column = db.Columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        //
        // GET: /Column/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Column/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Column column)
        {
            if (ModelState.IsValid)
            {
                db.Columns.Add(column);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(column);
        }

        //
        // GET: /Column/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Column column = db.Columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        //
        // POST: /Column/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Column column)
        {
            if (ModelState.IsValid)
            {
                db.Entry(column).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(column);
        }

        //
        // GET: /Column/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Column column = db.Columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        //
        // POST: /Column/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Column column = db.Columns.Find(id);
            db.Columns.Remove(column);
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