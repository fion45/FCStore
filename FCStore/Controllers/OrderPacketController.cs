using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Text.RegularExpressions;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class OrderPacketController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Delete(int id)
        {
            int removeIndex = id;
            bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
            HttpCookie cookie = null;
            string tmpStr = "";
            if (hasCookie)
            {
                cookie = Request.Cookies["Order"];
                tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(tmpStr);
                if (!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    Group gi = tmpMatch.Groups["ORDERID"];
                    int OrderID = int.Parse(gi.Value);
                    Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                    if (order != null && order.Packets != null && order.Packets.Count > removeIndex)
                    {
                        //删除
                        OrderPacket delOP = order.Packets[removeIndex];
                        db.OrderPackets.Remove(delOP);
                        db.SaveChanges();
                        tmpStr = tmpStr.Substring(0, tmpMatch.Groups["PID"].Captures[removeIndex].Index)
                            + tmpStr.Substring(tmpMatch.Groups["IMG"].Captures[removeIndex].Index + tmpMatch.Groups["IMG"].Captures[removeIndex].Length + 1);
                    }
                    else
                    {
                        hasCookie = false;
                    }
                }
                else
                {
                    //Cookie格式错误
                    hasCookie = false;
                }
            }
            if(!hasCookie && HttpContext.User.Identity.IsAuthenticated)
            {
                //禁用了cookie或者cookie格式错误
                //从用户获得其订单
                tmpStr = "";
                MyUser myUser = HttpContext.User as MyUser;
                if(myUser != null)
                {
                    Order order = db.Orders.OrderByDescending(r=>r.OID).FirstOrDefault(r => r.UID == myUser.UID && r.Status == (int)Order.EOrderStatus.OS_Init);
                    if (order != null && order.Packets != null && order.Packets.Count > removeIndex)
                    {
                        OrderPacket delOP = order.Packets[removeIndex];
                        db.OrderPackets.Remove(delOP);
                        db.SaveChanges();
                        tmpStr = order.GetCoookieStr();
                    }
                    //重新设置cookie
                    cookie = new HttpCookie("Order");
                    cookie.Expires = DateTime.Now.AddMonths(1);
                }
            }
            cookie.Value = Server.UrlEncode(tmpStr);
            Response.Cookies.Add(cookie);
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