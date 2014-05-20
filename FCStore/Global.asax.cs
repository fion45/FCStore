using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FCStore.Models;
using System.Data.Entity;
using System.Web.Security;
using System.Text.RegularExpressions;
using FCStore.Common;

namespace FCStore
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ////改变连接字符串里的|DataDirectory|
            //string tmpStr = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            //string dataDir = AppDomain.CurrentDomain.BaseDirectory;
            //if (dataDir.EndsWith(@"\bin\Debug\")
            //|| dataDir.EndsWith(@"\bin\Release\"))
            //{
            //    dataDir = System.IO.Directory.GetParent(dataDir).Parent.Parent.FullName + "\\App_Data";
            //    AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
            //}
            //Database.SetInitializer(
            //        new CreateDatabaseIfNotExists<FCStoreDbContext>());
            //using (var context = new FCStoreDbContext())
            //{
            //    context.Database.Initialize(true);
            //}

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //注册RouteDebug
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
        }

        protected void Application_Error(object s, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex.GetType().Name == "HttpException")
            {
                HttpException exception = (HttpException)ex;
                if (exception.GetHttpCode() == 404)
                {
                    Response.StatusCode = 404;
                }
            }
            Server.ClearError();
        }

        void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "" && authCookie.Value != null)
            {
                //对当前的cookie进行解密   
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                Regex rgx = new Regex("<USERID>(.+)</USERID><USERNAME>(.+)</USERNAME><RIDARR>(.+)</RIDARR><RNARR>(.+)</RNARR><PERMISSION>(.+)</PERMISSION>");
                Match tmpMatch = rgx.Match(authTicket.UserData);

                if (!string.IsNullOrEmpty(tmpMatch.Value) && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
                {
                    MyUser myUser = new MyUser(int.Parse(tmpMatch.Groups[1].Value), tmpMatch.Groups[2].Value, tmpMatch.Groups[3].Value, tmpMatch.Groups[4].Value, tmpMatch.Groups[5].Value);
                    HttpContext.Current.User = myUser;
                }
            }
        }
    }
}