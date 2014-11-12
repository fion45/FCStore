using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Web.Security;
using System.Text;
using System.Text.RegularExpressions;
using FCStore.Common;
using System.Net;
using System.IO;
using System.Drawing;
using FCStore.Filters;

namespace FCStore.Controllers
{
    public class UserController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public PartialViewResult _ContactList()
        {

            return PartialView(db.Users);
        }

        public void LoginSuccess(User user)
        {
            StringBuilder tmpRPStr = new StringBuilder("," + user.Permission + ",");
            StringBuilder tmpRIDStr = new StringBuilder(",");
            StringBuilder tmpRNStr = new StringBuilder(",");
            foreach (Role role in user.Roles)
            {
                tmpRIDStr.Append(role.RID + ",");
                tmpRNStr.Append(role.RoleName + ",");
                tmpRPStr.Append(role.Permission + ",");
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

            //设置cookie(不能合在一起，奇怪)
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(authCookie);

            authCookie = new HttpCookie("UserInfo");
            authCookie.Values.Add("UID", user.UID.ToString());
            authCookie.Values.Add("UserName", user.UserName);
            authCookie.Values.Add("RID", tmpRIDStr.ToString());
            authCookie.Values.Add("Permission", tmpRPStr.ToString());
            authCookie.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Add(authCookie);
            ViewBag.LoginFail = 0;

            //把购物车的东西给予该用户
            bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
            HttpCookie cookie = null;
            if (hasCookie)
            {
                hasCookie = false;
                cookie = Request.Cookies["Order"];
                tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(ProductController.ORDERCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(tmpStr);
                if (!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    Group gi = tmpMatch.Groups["ORDERID"];
                    int OrderID = int.Parse(gi.Value);
                    Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                    hasCookie = order != null && order.Packets.Count > 0;
                }
            }
            if (!hasCookie)
            {
                //从数据库里取出最后的未完成的购物任务
                Order order = db.Orders.OrderByDescending(r => r.OID).FirstOrDefault(r => r.UID == user.UID && r.Status == (int)Order.EOrderStatus.OS_Init);
                if (order != null)
                {
                    cookie = new HttpCookie("Order");
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.Value = Server.UrlEncode(order.GetCoookieStr());
                    Response.Cookies.Add(cookie);
                }
            }
            //处理收藏夹的东西
            hasCookie = Request.Cookies.AllKeys.Contains("Keeps");
            string KStr = "";
            cookie = null;
            List<int> PIDArr = new List<int>();
            if (hasCookie)
            {
                cookie = Request.Cookies["Keeps"];
                tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(KeepController.KEEPCOOKIERGX);
                MatchCollection tmpMC = cookieRgx.Matches(tmpStr);
                if(tmpMC.Count > 0)
                {
                    foreach (Match tmpMatch in tmpMC)
                    {
                        if (!string.IsNullOrEmpty(tmpMatch.Value))
                        {
                            Group gi = tmpMatch.Groups["PRODUCTID"];
                            int PID = -1;
                            Keep inDBKeep = null;
                            if (int.TryParse(gi.Value, out PID))
                            {
                                inDBKeep = db.Keeps.FirstOrDefault(r => r.PID == PID && r.UID == user.UID);
                                KStr += tmpMatch.Groups["PRODUCTID"] + "," + tmpMatch.Groups["TITLE"] + "," + tmpMatch.Groups["IMG"] + ",";
                                if (inDBKeep != null)
                                {
                                    PIDArr.Add(inDBKeep.PID);
                                }
                                else
                                {
                                    Keep keep = new Keep();
                                    keep.PID = PID;
                                    keep.UID = user.UID;
                                    keep.LastDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                    db.Keeps.Add(keep);
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                cookie = new HttpCookie("Keeps");
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            List<Keep> keepArr = db.Keeps.Where(r => r.UID == user.UID && !PIDArr.Contains(r.PID)).ToList();
            //从数据库里取出该用户的收藏夹
            foreach (Keep item in keepArr)
            {
                if (item.Product != null)
                    KStr += item.Product.PID + "," + item.Product.Title.Substring(0, Math.Min(20, item.Product.Title.Length)) + "," + item.Product.ImgPathArr[0] + ",";
            }
            cookie.Value = Server.UrlEncode(KStr);
            Response.Cookies.Add(cookie);
        }

        [LoginActionFilterAttribute(beforeTag = 0)]
        public ActionResult Login(string userID, string PSW, string checkCode)
        {
            //防止暴力破解
            //设置COOKIE过期
            if (Response.Cookies.AllKeys.Contains(FormsAuthentication.FormsCookieName))
            {
                Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.MinValue;
            }

            if (userID == null || PSW == null)
            {
                return RedirectToAction("Login","Home");
            }
            User user = null;
            if (Session["LPTAG"] != null && int.Parse(Session["LPTAG"].ToString()) == -1 
                && (Session["Validate_code"] == null || (Session["Validate_code"] != null && checkCode != (Session["Validate_code"].ToString()))))
            {
                //ViewBag.LoginFail = -2;
                string jsonStr = PubFunction.BuildResult(user, Session["LPTAG"].ToString(), false, -2, "验证码错误");
                return Content(jsonStr);
            }
            else if (Session["LPTAG"] != null && int.Parse(Session["LPTAG"].ToString()) == -2)
            {
                //ViewBag.LoginFail = -3;
                string jsonStr = PubFunction.BuildResult(user, Session["LPTAG"].ToString(), false, -3, "登陆异常");
                return Content(jsonStr);
            }
            else
            {
                user = db.Users.FirstOrDefault(r => (r.LoginID == userID && r.LoginPSW == PSW));
                if(user != null)
                {
                    LoginSuccess(user);
                }
                else
                {
                    //ViewBag.LoginFail = -1;
                    string tmpStr = Session["LPTAG"] != null ? Session["LPTAG"].ToString() : null;
                    string jsonStr = PubFunction.BuildResult(user, tmpStr, false, -1, "用户名或密码错误");
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
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(psw) || string.IsNullOrEmpty(checkCode) || string.IsNullOrEmpty(Session["Validate_code"].ToString()))
            {
                if (Request.IsAjaxRequest())
                {
                    string jsonStr = PubFunction.BuildResult("err", null, false, -4);
                    return Content(jsonStr);
                }
                else
                {
                    return RedirectToAction("Register", "Home");
                }
            }
            User user = null;
            if (checkCode != (Session["Validate_code"].ToString()))
            {
                ViewBag.LoginFail = -1;
                return Content(PubFunction.BuildResult(null,null, false, -1, "验证码错误"));
            }
            else
            {
                user = db.Users.FirstOrDefault(r => (r.UserName == userName || r.Email == email));
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
                        Sex = false,
                        DefaultAddrID = null,
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
        public ActionResult LoginByWB(string wbId)
        {
            //判断该微博号是否已有账号关联
            User user = db.Users.FirstOrDefault(r => r.WBID == wbId);
            if (user != null)
            {
                LoginSuccess(user);
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(user);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult LoginByQQ(string openID, string accessToken)
        {
            //判断该QQ是否已有账号关联
            User user = db.Users.FirstOrDefault(r => r.QQOpenID == openID);
            if (user != null)
            {
                LoginSuccess(user);
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(user);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult RelativeUser(bool NTag, string LoginID, string PSW, string UserName, bool sex, string smallHead, string lagerHead, string openId, string accessToken, string wbId)
        {
            User user = null;
            bool gfTag = false;
            if (NTag)
            {
                //新用户,判断该用户名是否已存在
                user = db.Users.FirstOrDefault(r => r.LoginID == LoginID);
                if(user == null)
                {
                    user = new User()
                    {
                        LoginID = LoginID,
                        LoginPSW = PSW,
                        UserName = UserName,
                        Sex = sex
                    };
                    if(string.IsNullOrEmpty(wbId))
                    {
                        user.WBID = wbId;
                    }
                    else
                    {
                        user.QQOpenID = openId;
                        user.QQAccessToken = accessToken;
                    }
                    db.Users.Add(user);

                    //关联角色
                    Role role = db.Roles.FirstOrDefault(r => r.RID == (int)Role.RoleTypeID.RT_CLIENT);
                    if (role.Users == null)
                    {
                        role.Users = new List<User>();
                    }
                    role.Users.Add(user);
                    db.SaveChanges();
                    gfTag = true;
                }
            }
            else
            {
                //旧用户
                user = db.Users.FirstOrDefault(r => r.LoginID == LoginID && r.LoginPSW == PSW);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(wbId))
                    {
                        user.WBID = wbId;
                    }
                    else
                    {
                        user.QQOpenID = openId;
                        user.QQAccessToken = accessToken;
                    }
                    db.SaveChanges();
                    gfTag = true;
                }
            }
            if(gfTag)
            {
                try
                {
                    //获取用户图片
                    Uri uri = new Uri(smallHead);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    Bitmap sourcebm = new Bitmap(resStream);//初始化Bitmap图片
                    PubFunction.SaveImg(sourcebm, 40, 40, Server.MapPath(user.HeadPictureFilePath_S));

                    uri = new Uri(lagerHead);
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    response = (HttpWebResponse)request.GetResponse();
                    resStream = response.GetResponseStream();
                    sourcebm = new Bitmap(resStream);
                    PubFunction.SaveImg(sourcebm, 100, 100, Server.MapPath(user.HeadPictureFilePath));
                }
                catch
                {
                    //保存头像失败
                }
            }
            if (gfTag)
                LoginSuccess(user);
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(gfTag ? user : null);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult JustLogin(string UserName, bool sex, string smallHead, string lagerHead, string openId, string accessToken,string wbId)
        {
            User user = null;
            if(string.IsNullOrEmpty(wbId))
            {
                //QQ登陆
                user = db.Users.FirstOrDefault(r => r.QQOpenID == openId);
            }
            else
            {
                //微博登陆
                user = db.Users.FirstOrDefault(r => r.WBID == wbId);
            }
            if (user == null)
            {
                user = new User()
                {
                    UserName = UserName,
                    Sex = sex,
                    QQOpenID = openId,
                    QQAccessToken = accessToken,
                    WBID = wbId,
                    Gift = 0
                };
                db.Users.Add(user);

                //关联角色
                Role role = db.Roles.FirstOrDefault(r => r.RID == (int)Role.RoleTypeID.RT_CLIENT);
                if (role.Users == null)
                {
                    role.Users = new List<User>();
                }
                role.Users.Add(user);
                db.SaveChanges();
                try
                { 
                    //获取用户图片
                    Uri uri = new Uri(smallHead);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    Bitmap sourcebm = new Bitmap(resStream);//初始化Bitmap图片
                    PubFunction.SaveImg(sourcebm, 40, 40, Server.MapPath(user.HeadPictureFilePath_S));

                    uri = new Uri(lagerHead);
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    response = (HttpWebResponse)request.GetResponse();
                    resStream = response.GetResponseStream();
                    sourcebm = new Bitmap(resStream);
                    PubFunction.SaveImg(sourcebm, 100, 100, Server.MapPath(user.HeadPictureFilePath));
                }
                catch
                {
                    //获取头像失败
                }
            }
            LoginSuccess(user);
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(user);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        [MyAuthorizeAttribute]
        public ActionResult Details()
        {
            MyUser tmpUser = HttpContext.User as MyUser;
            User user = null;
            if (tmpUser != null)
            {
                user = db.Users.Find(new object[] { tmpUser.UID });
            }
            UserDetailsVM viewModel = new UserDetailsVM();
            viewModel.User = user;
            int RecentViewCount = 5;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["RecentViewCount"].ToString(), out RecentViewCount);
            viewModel.RecentViewArr = (from RV in db.RecentViews
                                       where RV.UID == user.UID
                                       orderby RV.RVID descending
                                       select RV).Take(RecentViewCount).ToList();
            int pushCount = 5;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["PushCount"].ToString(),out pushCount);
            List<PushInfo> tmpPIArr = viewModel.User.PushInfos;
            if(pushCount - tmpPIArr.Count > 0)
            {
                //根据其最近浏览的品牌和类型提供推送
                IEnumerable<int> CIDArr = from RV in viewModel.RecentViewArr
                                            select RV.Product.CID;
                IEnumerable<int> BIDArr = from RV in viewModel.RecentViewArr
                                            select RV.Product.BID;
                List<PushInfo> associatePI = (from PI in db.PushInfos
                                             where (PI.CID != null && CIDArr.Contains((int)PI.CID)) || (PI.BID != null && BIDArr.Contains((int)PI.BID))
                                             select PI).ToList();
                if(pushCount - tmpPIArr.Count > 0)
                {
                    associatePI.AddRange((from PI in db.PushInfos
                                          select PI).Take(pushCount - tmpPIArr.Count).ToList());
                }
                tmpPIArr.AddRange(associatePI);
            }
            viewModel.PushInfoArr = tmpPIArr;
            viewModel.OrderArr = (from order in db.Orders
                                  where order.UID == user.UID
                                  select order).Take(5).ToList();
            return View(viewModel);
        }

        public ActionResult Exit()
        {
            //退出，设置Cookie超时和User
            HttpCookie authCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
                authCookie.Expires = DateTime.MinValue;

            return Redirect("/");
        }

        [MyAuthorizeAttribute]
        public ActionResult SelectDefaultAddress(int id)
        {
            //修改用户默认地址
            MyUser user = HttpContext.User as MyUser;
            if (HttpContext.User.Identity.IsAuthenticated && user != null)
            {
                User client = db.Users.FirstOrDefault(r => r.UID == user.UID);
                client.DefaultAddrID = id;
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
        public ActionResult AddAddress(Address address)
        {
            db.Addresses.Add(address);
            MyUser tmpUser = HttpContext.User as MyUser;
            User user = null;
            if (tmpUser != null)
            {
                user = db.Users.Find(new object[] { tmpUser.UID });
            }
            user.DefaultAddress = address;
            user.Addresses.Add(address);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(address);
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