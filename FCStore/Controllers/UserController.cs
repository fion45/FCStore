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
            //设置COOKIE过期
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.MinValue;

            if (userID == null || PSW == null || checkCode == null || Session["Validate_code"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            User user = null;
            if (checkCode != (Session["Validate_code"].ToString()))
            {
                ViewBag.LoginFail = -2;
                string jsonStr = PubFunction.BuildResult(user,null ,false, -2, "验证码错误");
                return Content(jsonStr);
            }
            else
            {
                try
                {
                    user = db.Users.First(r => (r.LoginID == userID && r.LoginPSW == PSW));
                    StringBuilder tmpRPStr = new StringBuilder("," + user.Permission + ",");
                    StringBuilder tmpRIDStr = new StringBuilder(",");
                    StringBuilder tmpRNStr = new StringBuilder(",");
                    foreach (Role role in user.Roles)
                    {
                        tmpRIDStr.Append(role.RID + ",");
                        tmpRNStr.Append(role.RoleName + ",");
                        tmpRPStr.Append(role.Permission + ",");
                    }
                    string tmpStr = string.Format("<USERID>{0}</USERID><USERNAME>{1}</USERNAME><RIDARR>{2}</RIDARR><RNARR>{3}</RNARR><PERMISSION>{4}</PERMISSION>", user.UID,user.UserName,tmpRIDStr.ToString(), tmpRNStr.ToString(), tmpRPStr.ToString());

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                       1,
                       user.UserName,
                       DateTime.Now,
                       DateTime.Now.AddMinutes(30),
                       true,
                       tmpStr);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //设置cookie(不能合在一起，奇怪)
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);

                    authCookie = new HttpCookie("UserInfo");
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
                    string jsonStr = PubFunction.BuildResult(user,null, false, -1, "用户名或密码错误");
                    return Content(jsonStr);
                }
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(user);
                return Content(jsonStr);
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult Register(string userName, string email, string psw, string checkCode)
        {
            if (userName == null || email == null || psw == null || checkCode == null || Session["Validate_code"] == null)
            {
                return RedirectToAction("Register", "Home");
            }
            User user = null;
            if (checkCode != (Session["Validate_code"].ToString()))
            {
                ViewBag.LoginFail = -1;
                return Content(PubFunction.BuildResult(null,null, false, -1, "验证码错误"));
            }
            else
            {
                try
                {
                    user = db.Users.First(r => (r.UserName == userName || r.Email == email));
                }
                catch
                {
                    user = null;
                }
                if (user != null)
                {
                    //用户已存在
                    bool UNExists = user.UserName == userName;
                    return Content(PubFunction.BuildResult(null,null, false, UNExists ? -2 : -3, UNExists ? "用户名已被注册" : "邮箱已被注册"));
                }
                else
                {
                    //创建用户
                    user = new User()
                    {
                        UserName = userName,
                        LoginID = userName,
                        LoginPSW = psw,
                        Email = email,
                        Permission = "",
                        Gift = 100      //100积分
                    };
                    user = db.Users.Add(user);
                    //关联角色
                    Role role = db.Roles.First(r => r.RID == (int)Role.RoleTypeID.RT_CLIENT);
                    if (role.Users == null)
                    {
                        role.Users = new List<User>();
                    }
                    role.Users.Add(user);
                    db.SaveChanges();
                    //设置cookie
                    StringBuilder tmpRPStr = new StringBuilder("," + user.Permission + ",");
                    StringBuilder tmpRIDStr = new StringBuilder(",");
                    StringBuilder tmpRNStr = new StringBuilder(",");
                    foreach (Role tmpRole in user.Roles)
                    {
                        tmpRIDStr.Append(tmpRole.RID + ",");
                        tmpRNStr.Append(tmpRole.RoleName + ",");
                        tmpRPStr.Append(tmpRole.Permission + ",");
                    }
                    string tmpStr = string.Format("<USERID>{0}</USERID><USERNAME>{1}</USERNAME><RIDARR>{2}</RIDARR><RNARR>{3}</RNARR><PERMISSION>{4}</PERMISSION>", user.UID, user.UserName, tmpRIDStr.ToString(), tmpRNStr.ToString(), tmpRPStr.ToString());

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                       1,
                       user.UserName,
                       DateTime.Now,
                       DateTime.Now.AddMinutes(30),
                       true,
                       tmpStr);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);

                    authCookie = new HttpCookie("UserInfo");
                    authCookie.Values.Add("UID", user.UID.ToString());
                    authCookie.Values.Add("UserName", user.UserName);
                    authCookie.Values.Add("RID", tmpRIDStr.ToString());
                    authCookie.Values.Add("Permission", tmpRPStr.ToString());
                    Response.Cookies.Add(authCookie);
                }
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(user);
                return Content(jsonStr);
            }
            else
            {
                return View(user);
            }
        }

        [MyAuthorizeAttribute]
        public ActionResult Details()
        {
            MyUser tmpUser = HttpContext.User as MyUser;
            User user = null;
            if (tmpUser == null)
            {
                throw new Exception("User error");
            }
            else
            {
                user = db.Users.Find(new object[] { tmpUser.UID });
            }
            return View(user);
        }

        public ActionResult Exit()
        {
            //退出，设置Cookie超时和User
            HttpCookie authCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
            authCookie.Expires = DateTime.MinValue;

            return Redirect("/");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}