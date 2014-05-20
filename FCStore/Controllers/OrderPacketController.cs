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
    public class OrderPacketController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        //
        // GET: /OrderPacket/

        public ActionResult Index()
        {
            var orderpackets = db.OrderPackets.Include(o => o.Product);
            return View(orderpackets.ToList());
        }

        //
        // GET: /OrderPacket/Details/5

        public ActionResult Details(int id = 0)
        {
            OrderPacket orderpacket = db.OrderPackets.Find(id);
            if (orderpacket == null)
            {
                return HttpNotFound();
            }
            return View(orderpacket);
        }

        //
        // GET: /OrderPacket/Create

        public ActionResult Create()
        {
            ViewBag.PID = new SelectList(db.Products, "PID", "Title");
            return View();
        }

        //
        // POST: /OrderPacket/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderPacket orderpacket)
        {
            if (ModelState.IsValid)
            {
                db.OrderPackets.Add(orderpacket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PID = new SelectList(db.Products, "PID", "Title", orderpacket.PID);
            return View(orderpacket);
        }

        //
        // GET: /OrderPacket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            OrderPacket orderpacket = db.OrderPackets.Find(id);
            if (orderpacket == null)
            {
                return HttpNotFound();
            }
            ViewBag.PID = new SelectList(db.Products, "PID", "Title", orderpacket.PID);
            return View(orderpacket);
        }

        //
        // POST: /OrderPacket/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderPacket orderpacket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderpacket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PID = new SelectList(db.Products, "PID", "Title", orderpacket.PID);
            return View(orderpacket);
        }

        //
        // GET: /OrderPacket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            OrderPacket orderpacket = db.OrderPackets.Find(id);
            if (orderpacket == null)
            {
                return HttpNotFound();
            }
            return View(orderpacket);
        }

        //
        // POST: /OrderPacket/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderPacket orderpacket = db.OrderPackets.Find(id);
            db.OrderPackets.Remove(orderpacket);
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