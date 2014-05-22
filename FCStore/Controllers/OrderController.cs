using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Text.RegularExpressions;
using FCStore.FilterAttribute;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class OrderController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        [MyAuthorizeAttribute]
        public ActionResult Cart()
        {
            OrderVM tmpVM = new OrderVM();
            //设置订单为其用户的订单
            MyUser user = HttpContext.User as MyUser;
            if (HttpContext.User.Identity.IsAuthenticated && user != null)
            {
                tmpVM.Client = db.Users.FirstOrDefault(r => r.UID == user.UID);
                tmpVM.OrderArr = new List<Order>();
                bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
                HttpCookie cookie = null;
                if(hasCookie)
                {
                    cookie = Request.Cookies["Order"];
                    string tmpStr = Server.UrlDecode(cookie.Value);
                    Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
                    Match tmpMatch = cookieRgx.Match(tmpStr);
                    if (!string.IsNullOrEmpty(tmpMatch.Value))
                    {
                        Group gi = tmpMatch.Groups["ORDERID"];
                        int OrderID = int.Parse(gi.Value);
                        Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                        if (order != null)
                        {
                            order.UID = user.UID;
                            db.SaveChanges();
                            tmpVM.OrderArr.Add(order);
                        }
                    }
                }
                List<Order> tmpOArr = db.Orders.Where(r => (r.UID == user.UID && r.Status == (int)Order.EOrderStatus.OS_Init)).ToList();
                tmpVM.OrderArr.AddRange(tmpOArr);
            }
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult Payment()
        {
            //加载用户的地址

            return View();
        }

        [MyAuthorizeAttribute]
        public ActionResult Submit()
        {

            return View();
        }

        //
        // GET: /Order/

        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.User);
            return View(orders.ToList());
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // GET: /Order/Create

        public ActionResult Create()
        {
            ViewBag.UID = new SelectList(db.Users, "UID", "UserName");
            return View();
        }

        //
        // POST: /Order/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UID = new SelectList(db.Users, "UID", "UserName", order.UID);
            return View(order);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UID = new SelectList(db.Users, "UID", "UserName", order.UID);
            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UID = new SelectList(db.Users, "UID", "UserName", order.UID);
            return View(order);
        }

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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