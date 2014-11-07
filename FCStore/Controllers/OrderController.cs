using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Text.RegularExpressions;
using FCStore.Filters;
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
                            if (order.Status >= (int)Order.EOrderStatus.OS_Subscription)
                            {
                                //已支付，不能修改订单，不能重新支付
                                return Redirect("/Order/HasPayed/" + OrderID);
                            }
                            else if (order.Status == (int)Order.EOrderStatus.OS_Order)
                            {
                                //已确认订单，不能修改了，只能整个删除
                                return Redirect("/Order/Submit");
                            }
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
            //转到支付宝页面，进行支付
            return View();
        }
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

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        [HttpGet, ValidateInput(false)]
        public ActionResult AlipayCB()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();
            Order orderPtr = null;
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    //商户订单号
                    string out_trade_no = Request.QueryString["out_trade_no"];

                    string OIDStr = out_trade_no.Substring(out_trade_no.Length - 8);
                    int OID = Convert.ToInt32(OIDStr, 16);

                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];


                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];
                    orderPtr = db.Orders.FirstOrDefault(r => r.OID == OID);
                    switch(trade_status)
                    {
                        case "WAIT_SELLER_SEND_GOODS":
                            {
                                //付款成功
                                if (orderPtr.Status >= (int)Order.EOrderStatus.OS_Payment)
                                {
                                    //已支付
                                }
                                else
                                {
                                    //判断价钱是否正确
                                    string tmpStr = Request.QueryString["price"];
                                    decimal price = decimal.Parse(tmpStr);
                                    tmpStr = Request.QueryString["quantity"];
                                    int count = int.Parse(tmpStr);
                                    tmpStr = Request.QueryString["logistics_fee"];
                                    decimal fee = decimal.Parse(tmpStr);
                                    if (price == orderPtr.PayAmount && count == 1 && fee == orderPtr.Postage)
                                    {
                                        orderPtr.Status = (int)Order.EOrderStatus.OS_Payment;
                                        //保存淘宝交易号
                                        orderPtr.AP_TradeNO = trade_no;
                                        orderPtr.PayDate = Request.QueryString["notify_time"];
                                    }
                                    else
                                    {
                                        //支付信息有出入
                                        orderPtr.Status = (int)Order.EOrderStatus.OS_ERR_PAYMENT;
                                    }
                                    db.SaveChanges();
                                }
                                return Redirect("/Order/HasPayed/" + OID);
                                break;
                            }
                        case "TRADE_FINISHED":
                            {
                                //交易完成
                                if (orderPtr.AP_TradeNO != trade_no)
                                {
                                    //支付宝单号不一致

                                }
                                if (orderPtr.Status >= (int)Order.EOrderStatus.OS_Complete)
                                {
                                    //已完成
                                }
                                else
                                {
                                    orderPtr.Status = (int)Order.EOrderStatus.OS_Complete;
                                    orderPtr.CompleteDate = DateTime.Now.ToString(PubFunction.LONGDATETIMEFORMAT);
                                    db.SaveChanges();
                                }
                                break;
                            }
                        case "WAIT_BUYER_PAY":
                            {
                                //等待买家付款
                                if (orderPtr.Status != (int)Order.EOrderStatus.OS_Order && orderPtr.Status < (int)Order.EOrderStatus.OS_Payment)
                                {
                                    orderPtr.Status = (int)Order.EOrderStatus.OS_Order;
                                    db.SaveChanges();
                                }
                                break;
                            }
                        case "WAIT_BUYER_CONFIRM_GOODS":
                            {
                                if (orderPtr.AP_TradeNO != trade_no)
                                {
                                    //支付宝单号不一致

                                }
                                if (orderPtr.Status >= (int)Order.EOrderStatus.OS_InlandSending)
                                {
                                    //已发货
                                }
                                else
                                {
                                    orderPtr.Status = (int)Order.EOrderStatus.OS_InlandSending;
                                    db.SaveChanges();
                                }
                                break;
                            }
                        case "TRADE_CLOSED":
                            {
                                //无故关闭交易过程
                                
                                if (orderPtr.AP_TradeNO != trade_no)
                                {
                                    //支付宝单号不一致

                                }
                                if (orderPtr.Status != (int)Order.EOrderStatus.OS_ERR_Complete)
                                {
                                    orderPtr.Status = (int)Order.EOrderStatus.OS_ERR_Complete;
                                    db.SaveChanges();
                                }
                                break;
                            }
                    }

                }
            }
            return View(orderPtr);
        }

        [MyAuthorizeAttribute]
        public ActionResult Submit()
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
                            if (order.Status >= (int)Order.EOrderStatus.OS_Subscription)
                            {
                                return Redirect("/Order/HasPayed/" + OrderID);
                            }
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
                if (tmpVM.OrderArr.Count == 0)
                {
                    return Redirect("/Order/Cart");
                }
            }
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult HasPayed(int id)
        {
            Order orderPtr = null;
            MyUser user = HttpContext.User as MyUser;
            if (HttpContext.User.Identity.IsAuthenticated && user != null)
            {
                orderPtr = db.Orders.FirstOrDefault(r => r.OID == id);
                if (orderPtr != null && orderPtr.Status == (int)Order.EOrderStatus.OS_Payment)
                {
                    if(orderPtr.UID != user.UID)
                    {
                        orderPtr = null;
                    }
                }
                else
                {
                    //orderPtr = null;
                }
            }
            if (orderPtr == null)
            {
                return Redirect("/Order/Cart");
            }
            else
            {
                HttpCookie cookie = Request.Cookies["Order"];
                if (null != cookie)
                {
                    //设置OrderCookie超时
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cookie);
                }
                return View(orderPtr);
            }
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
            string BDT, EDT = tmpDT.ToString(DTFormat);
            SaleLogVM tmpSLVM = new SaleLogVM();
            tmpSLVM.DTStrArr = new List<string>();
            tmpSLVM.CountArr = new List<int>();
            tmpSLVM.BDTStrArr = new List<string>();
            tmpSLVM.EDTStrArr = new List<string>();
            tmpSLVM.ShamCountArr = new List<int>();
            for(int i=0;i<4;i++)
            {
                tmpDT = tmpDT.AddDays(-7);
                BDT = tmpDT.ToString(DTFormat);
                BDTStr = tmpDT.ToString(DTF);
                var opArr = from op in tmpOPLST
                            where op.Order.OrderDate.CompareTo(BDT) > 0 && op.Order.OrderDate.CompareTo(EDT) <= 0
                                            select op;
                int shamCount = db.ShamOrderDatas.Count(r => r.Product.PID == ID && r.DateTime.CompareTo(BDT) > 0 && r.DateTime.CompareTo(EDT) <= 0);
                tmpSLVM.BDTStrArr.Insert(0, BDT);
                tmpSLVM.EDTStrArr.Insert(0, EDT);
                string tmpDTStr = string.Format("\"{0}\" 至 \"{1}\"",BDTStr,EDTStr);
                tmpSLVM.DTStrArr.Insert(0,tmpDTStr);
                tmpSLVM.CountArr.Insert(0, opArr.Sum(r => r.Count));
                tmpSLVM.ShamCountArr.Insert(0, shamCount);
                EDT = BDT;
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