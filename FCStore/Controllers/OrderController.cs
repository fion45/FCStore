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

        public ActionResult DeletePacket(string id)
        {
            string[] tmpIDArr = id.Split(new char[] { ',' });
            int OID = int.Parse(tmpIDArr[0]);
            Order tmpOrder = db.Orders.FirstOrDefault(r => r.OID == OID);
            if(tmpOrder != null)
            {
                string tmpStr = "";
                for (int i = 1; i < tmpIDArr.Length;i++ )
                {
                    tmpStr = tmpIDArr[i];
                    int PID = int.Parse(tmpStr);
                    tmpOrder.Packets.RemoveAll(r => r.PacketID == PID);
                }
                db.SaveChanges();
                //添加到cookie里
                bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
                HttpCookie cookie = null;
                if (!hasCookie)
                {
                    cookie = new HttpCookie("Order");
                    cookie.Expires = DateTime.Now.AddMonths(1);
                }
                else
                {
                    cookie = Request.Cookies["Order"];
                }
                tmpStr = tmpOrder.OID.ToString() + ",";
                foreach(OrderPacket op in tmpOrder.Packets)
                {
                    tmpStr = op.Product.PID + "," + op.Count.ToString() + "," + op.Product.Title.Substring(0, Math.Min(20, op.Product.Title.Length)) + "," + op.Product.ImgPathArr[0] + ",";
                }
                cookie.Value = Server.UrlEncode(tmpStr);
                Response.Cookies.Add(cookie);
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

        [MyAuthorizeAttribute]
        public ActionResult CancelOrder(int id)
        {
            //删除Cookie
            bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
            if (hasCookie)
            {
                Response.Cookies["Order"].Expires = DateTime.MinValue;
            }
            
            //修改数据库Order的状态
            Order order = db.Orders.FirstOrDefault(r => r.OID == id);
            if (order != null)
            {
                db.Entry(order).State = EntityState.Deleted;
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

        [MyAuthorizeAttribute]
        public ActionResult SubmitOrder(SubmitObj order)
        {

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}