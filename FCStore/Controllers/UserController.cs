using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Web.Security;
using FCStore.FilterAttribute;
using System.Text;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class UserController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Login(string userID, string PSW, string checkCode)
        {
            if (userID == null || PSW == null || checkCode == null || Session["Validate_code"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            User user = null;
            if (checkCode != (Session["Validate_code"].ToString()))
            {
                ViewBag.LoginFail = -2;
            }
            else
            {
                try
                {
                    user = db.Users.Where(r => (r.LoginID == userID && r.LoginPSW == PSW)).First();
                    StringBuilder tmpRPStr = new StringBuilder("," + user.Permission + ",");
                    StringBuilder tmpRIDStr = new StringBuilder(",");
                    StringBuilder tmpRNStr = new StringBuilder(",");
                    foreach (Role role in user.Roles)
                    {
                        tmpRIDStr.Append(role.RID + ",");
                        tmpRNStr.Append(role.RoleName + ",");
                        tmpRPStr.Append(role.Permission + ",");
                    }
                    string tmpStr = string.Format("<RIDARR>{0}</RIDARR><RNARR>{1}</RNARR><PERMISSION>{2}</PERMISSION>", tmpRIDStr.ToString(), tmpRNStr.ToString(), tmpRPStr.ToString());

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                       1,
                       user.UserName,
                       DateTime.Now,
                       DateTime.Now.AddMinutes(30),
                       true,
                       tmpStr);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    //设置cookie
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    authCookie.Values.Add("UID", user.UID.ToString());
                    authCookie.Values.Add("UserName", user.UserName);
                    authCookie.Values.Add("RID", tmpRIDStr.ToString());
                    authCookie.Values.Add("Permission", tmpRPStr.ToString());
                    Response.Cookies.Add(authCookie);
                    ViewBag.LoginFail = 0;
                }
                catch
                {
                    ViewBag.LoginFail = -1;
                    user = null;
                }
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(user);
                return Content(jsonStr);
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult Registe(User user)
        {
            return View(user);
        }

        [MyAuthorizeAttribute]
        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}