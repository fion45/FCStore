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
using System.Collections.Specialized;
using Com.Alipay;

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
                if(tmpVM.Client.DefaultAddress == null && tmpVM.Client.Addresses.Count > 0)
                    tmpVM.Client.DefaultAddress = tmpVM.Client.Addresses.First();
                tmpVM.OrderArr = new List<Order>();
                bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
                HttpCookie cookie = null;
                int OrderID = -1;
                if(hasCookie)
                {
                    cookie = Request.Cookies["Order"];
                    string tmpStr = Server.UrlDecode(cookie.Value);
                    Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
                    Match tmpMatch = cookieRgx.Match(tmpStr);
                    if (!string.IsNullOrEmpty(tmpMatch.Value))
                    {
                        Group gi = tmpMatch.Groups["ORDERID"];
                        OrderID = int.Parse(gi.Value);
                        Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                        if (order != null)
                        {
                            order.UID = user.UID;
                            db.SaveChanges();
                            tmpVM.OrderArr.Add(order);
                        }
                    }
                }
                List<Order> tmpOArr = db.Orders.Where(r => (r.OID != OrderID && r.UID == user.UID && r.Status == (int)Order.EOrderStatus.OS_Init)).OrderByDescending(r => r.OrderDate).ToList();
                tmpVM.OrderArr.AddRange(tmpOArr);
            }
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult Payment()
        {
            return View();
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlipayCB()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号

                    string out_trade_no = Request.QueryString["out_trade_no"];

                    //支付宝交易号

                    string trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];


                    if (Request.QueryString["trade_status"] == "WAIT_SELLER_SEND_GOODS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                    }
                    else if (Request.QueryString["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                    }
                    else
                    {
                        Response.Write("trade_status=" + Request.QueryString["trade_status"]);
                    }

                    //打印页面
                    Response.Write("验证成功<br />");
                    Response.Write("trade_no=" + trade_no);

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Response.Write("验证失败");
                }
            }
            else
            {
                Response.Write("无返回参数");
            }

            return View();
        }

        [MyAuthorizeAttribute]
        public ActionResult Submit(int id)
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
                int OrderID = -1;
                if (hasCookie)
                {
                    cookie = Request.Cookies["Order"];
                    string tmpStr = Server.UrlDecode(cookie.Value);
                    Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
                    Match tmpMatch = cookieRgx.Match(tmpStr);
                    if (!string.IsNullOrEmpty(tmpMatch.Value))
                    {
                        Group gi = tmpMatch.Groups["ORDERID"];
                        OrderID = int.Parse(gi.Value);
                        Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                        if (order != null)
                        {
                            order.UID = user.UID;
                            order.Contacts = tmpVM.Client.DefaultAddress.Contacts;
                            order.TownID = tmpVM.Client.DefaultAddress.TownID;
                            order.BelongTown = tmpVM.Client.DefaultAddress.BelongTown;
                            order.AddressName = tmpVM.Client.DefaultAddress.AddressName;
                            order.Phone = tmpVM.Client.DefaultAddress.Phone;
                            order.PostCode = tmpVM.Client.DefaultAddress.PostCode;
                            db.SaveChanges();
                            tmpVM.OrderArr.Add(order);
                        }
                    }
                }
                List<Order> tmpOArr = db.Orders.Where(r => (r.OID != OrderID && r.UID == user.UID && r.Status == (int)Order.EOrderStatus.OS_Init)).OrderByDescending(r => r.OrderDate).ToList();
                tmpVM.OrderArr.AddRange(tmpOArr);
            }
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
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
        public ActionResult SubmitOrder(int OrderID,int SendType,PacketObj[] packets)
        {
            string tmpResult = "OK";
            try
            {
                //提交订单
                MyUser tmpUser = HttpContext.User as MyUser;
                if (tmpUser != null)
                {
                    Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID && r.UID == tmpUser.UID);
                    if(order != null)
                    {
                        order.SendType = SendType;
                        foreach(OrderPacket packet in order.Packets)
                        {
                            PacketObj tmpPacket = packets.FirstOrDefault(r => r.PacketID == packet.PacketID);
                            if(tmpPacket == null)
                                throw new Exception("Order Packet is different!");
                            packet.Count = tmpPacket.Count;
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                tmpResult = "Err";
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(tmpResult);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult getSaleLogByPID(int ID)
        {
            //获得销售记录
            string DTFormat = "yyyy-MM-dd hh:mm:ss";
            string DTF = "M月d日";
            int[] NCStatus = new int[] { (int)Order.EOrderStatus.OS_Init };
            string DTStr = DateTime.Now.AddMonths(-1).ToString(DTFormat);
            List<OrderPacket> tmpOPLST = db.OrderPackets.Where(r => r.PID == ID && !NCStatus.Contains(r.Order.Status) &&  r.Order.OrderDate.CompareTo(DTStr) > 0).ToList();
            DateTime tmpDT = DateTime.Now;
            string EDTStr = tmpDT.ToString(DTF);
            string BDTStr;
            SaleLogVM tmpSLVM = new SaleLogVM();
            tmpSLVM.DTStrArr = new List<string>();
            tmpSLVM.CountArr = new List<int>();
            for(int i=0;i<4;i++)
            {
                tmpDT = tmpDT.AddDays(-7);
                BDTStr = tmpDT.ToString(DTF);
                var opArr = from op in tmpOPLST
                                          where op.Order.OrderDate.CompareTo(BDTStr) > 0 && op.Order.OrderDate.CompareTo(EDTStr) <= 0
                                          select op;
                string tmpDTStr = string.Format("\"{0}\" 至 \"{1}\"",BDTStr,EDTStr);
                tmpSLVM.DTStrArr.Insert(0,tmpDTStr);
                tmpSLVM.CountArr.Add(opArr.Sum(r => r.Count));
                EDTStr = BDTStr;
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(tmpSLVM);
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