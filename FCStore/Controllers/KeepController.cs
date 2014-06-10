using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using FCStore.Common;
using System.Text.RegularExpressions;

namespace FCStore.Controllers
{
    public class KeepController : Controller
    {
        static public string KEEPCOOKIERGX = "^(?<KITEM>(?<PRODUCTID>[^,]+?),(?<TITLE>[^,]+?),(?<IMG>[^,]+?),)*$";

        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Add(string id)
        {
            //保存到Cookie
            bool hasCookie = Request.Cookies.AllKeys.Contains("Keeps");
            HttpCookie cookie = null;
            string tmpStr = "";
            if (hasCookie)
            {
                cookie = Request.Cookies["Keeps"];
                tmpStr = Server.UrlDecode(cookie.Value);
            }
            else
            {
                cookie = new HttpCookie("Keeps");
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            string[] strArr = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string IDStr in strArr)
            {
                int PID = int.Parse(IDStr);
                Product product = db.Products.FirstOrDefault(r => r.PID == PID);
                if (product != null)
                {
                    bool eTag = true;
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        //已登录
                        MyUser tmpUser = HttpContext.User as MyUser;
                        if (tmpUser != null)
                        {
                            //登陆用户
                            Keep exsisKeep = db.Keeps.FirstOrDefault(r => r.PID == PID && r.UID == tmpUser.UID);
                            if (exsisKeep == null)
                            {
                                if(db.Keeps.Local.FirstOrDefault(r => r.PID == PID && r.UID == tmpUser.UID) == null)
                                {
                                    Keep keep = new Keep();
                                    keep.PID = PID;
                                    keep.UID = tmpUser.UID;
                                    keep.LastDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                    db.Keeps.Add(keep);
                                    eTag = false;
                                }
                            }
                            else
                            {
                                exsisKeep.LastDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        eTag = false;
                        Regex cookieRgx = new Regex(KEEPCOOKIERGX);
                        Match tmpMatch = cookieRgx.Match(tmpStr);
                        if (!string.IsNullOrEmpty(tmpMatch.Value))
                        {
                            int tmpC = tmpMatch.Groups["KITEM"].Captures.Count;
                            for (int i = 0; i < tmpC; i++)
                            {
                                if (int.Parse(tmpMatch.Groups["PRODUCTID"].Captures[i].Value) == PID)
                                { 
                                    eTag = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!eTag)
                        tmpStr += product.PID + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
                }
            }
            if (HttpContext.User.Identity.IsAuthenticated)
                db.SaveChanges();
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

        public ActionResult List()
        {
            List<Keep> keeps = new List<Keep>();
            bool hasCookie = Request.Cookies.AllKeys.Contains("Keeps");
            HttpCookie cookie = null;
            if (hasCookie)
            {
                cookie = Request.Cookies["Keeps"];
                string tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(KEEPCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(tmpStr);
                if (!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    int tmpC = tmpMatch.Groups["KITEM"].Captures.Count;
                    for (int i = 0; i < tmpC; i++)
                    {
                        Keep tmpK = new Keep();
                        int tmpPID = int.Parse(tmpMatch.Groups["PRODUCTID"].Captures[i].Value);
                        tmpK.Product = db.Products.FirstOrDefault(r => r.PID == tmpPID);
                        keeps.Add(tmpK);
                    }
                }
            }
            return View(keeps);
        }

        public ActionResult Buy(string id)
        {
            string[] tmpIDArr = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> PIDArr = new List<int>();
            foreach (string PIDStr in tmpIDArr)
            {
                int PID = int.Parse(PIDStr);
                PIDArr.Add(PID);
            }
            List<Product> products = db.Products.Where(r => PIDArr.Contains(r.PID)).ToList();
            string tmpStr = "";
            bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
            HttpCookie cookie = null;
            Order order = null;
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
                    order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                }
            }
            else
            {
                cookie = new HttpCookie("Order");
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            foreach(Product product in products)
            {
                OrderPacket packet = new OrderPacket();
                packet.PID = product.PID;
                packet.Product = product;
                packet.Univalence = product.Price;
                packet.Discount = product.Discount;
                packet.Count = 1;
                if(order == null)
                {
                    order = new Order();
                    order.Packets = new List<OrderPacket>();
                    order.UID = null;
                    order.Postage = 0;
                    order.Subscription = 0;
                    order.Status = (int)Order.EOrderStatus.OS_Init;
                    order.SendType = (int)Order.ESendType.ST_Direct;
                    order.PayType = (int)Order.EPayType.PT_Alipay;
                    order.OrderDate = null;
                    order.CompleteDate = null;
                    db.Orders.Add(order);
                }
                order.Packets.Add(packet);
                db.OrderPackets.Add(packet);
            }
            if(HttpContext.User.Identity.IsAuthenticated && order.UID == null)
            {
                //已登录
                MyUser tmpUser = HttpContext.User as MyUser;
                if (tmpUser != null)
                {
                    //登陆用户
                    order.UID = tmpUser.UID;
                }
            }
            order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            db.SaveChanges();
            tmpStr = order.OID.ToString() + ",";
            foreach (OrderPacket packet in order.Packets)
            {
                tmpStr += packet.Product.PID + ",1," + packet.Product.Title.Substring(0, Math.Min(20, packet.Product.Title.Length)) + "," + packet.Product.ImgPathArr[0] + ",";
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

        public ActionResult Delete(string id)
        {
            string[] tmpIDArr = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> PIDArr = new List<int>();
            foreach(string tmpStr in tmpIDArr)
            {
                int PID = int.Parse(tmpStr);
                PIDArr.Add(PID);
            }

            bool hasCookie = Request.Cookies.AllKeys.Contains("Keeps");
            HttpCookie cookie = null;
            if (!hasCookie)
            {
                cookie = new HttpCookie("Keeps");
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            else
            {
                cookie = Request.Cookies["Keeps"];
            }
            MyUser user = HttpContext.User as MyUser;
            if (HttpContext.User.Identity.IsAuthenticated && user != null)
            {
                List<Keep> keepLST = db.Keeps.Where(r => r.UID == user.UID).ToList();
                string tmpStr = "";
                foreach(Keep tmpKeep in keepLST)
                {
                    if (PIDArr.Contains(tmpKeep.PID))
                    {
                        db.Keeps.Remove(tmpKeep);
                    }
                    else
                    {
                        tmpStr += tmpKeep.Product.PID + "," + tmpKeep.Product.Title.Substring(0, Math.Min(20, tmpKeep.Product.Title.Length)) + "," + tmpKeep.Product.ImgPathArr[0] + ",";
                    }
                }
                db.SaveChanges();
                cookie.Value = Server.UrlEncode(tmpStr);
                Response.Cookies.Add(cookie);
            }
            else if(hasCookie)
            {
                string tmpStr = "";
                string cookieStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(KEEPCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(cookieStr);
                int tmpC = tmpMatch.Groups["KITEM"].Captures.Count;
                for (int i = 0; i < tmpC; i++)
                {
                    if (!PIDArr.Contains(int.Parse(tmpMatch.Groups["PRODUCTID"].Captures[i].Value)))
                    {
                        tmpStr += tmpMatch.Groups["PRODUCTID"].Captures[i].Value + "," + tmpMatch.Groups["TITLE"].Captures[i].Value + "," + tmpMatch.Groups["IMG"].Captures[i].Value + ",";
                    }
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}