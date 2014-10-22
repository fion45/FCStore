using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Text;
using FionPushFilm.Common;
using FionPushFilm.Models;
using FionPushFilm.Controllers;

namespace FionPushFilm
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object s, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null && ex.GetType().Name == "HttpException")
            {
                HttpException exception = (HttpException)ex;
                if (exception.GetHttpCode() == 404)
                {
                    Response.StatusCode = 404;
                }
            }
            Server.ClearError();
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_LogRequest(object sender, EventArgs e)
        {
            bool enableTrail = false;
            if (System.Configuration.ConfigurationManager.AppSettings["EnableTrail"] != null)
            {
                if (bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["EnableTrail"], out enableTrail) && enableTrail)
                {
                    ClientTrailController tmpCon = new ClientTrailController();
                    tmpCon.WriteTrail(this.Request);
                }
            }
        }
    }
}