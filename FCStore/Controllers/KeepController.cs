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
                    if (!eTag)
                        tmpStr += product.PID + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
                }
            }
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
                        tmpK.PID = int.Parse(tmpMatch.Groups["PRODUCTID"].Captures[i].Value);
                        tmpK.Product = new Product();
                        tmpK.Product.Title = tmpMatch.Groups["TITLE"].Captures[i].Value;
                        tmpK.Product.ImgPath = tmpMatch.Groups["IMG"].Captures[i].Value;
                        keeps.Add(tmpK);
                    }
                }
            }
            return View(keeps);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}