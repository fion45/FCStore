using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FCStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "buy",
                url: "{controller}/{action}/{id}/{count}",
                defaults: new { id = UrlParameter.Optional, count = 1 }
            );

            routes.MapRoute(
                name: "zone",
                url: "{controller}/{action}/{PID}/{CID}",
                defaults: new { PID = UrlParameter.Optional, CID = -1 }
            );

            routes.MapRoute(
                name: "ProductList",
                url: "{controller}/{action}/{id}/{pIndex}/{hashOrder}/{hashWhere}",
                defaults: new { id = UrlParameter.Optional, pIndex = 1, hashOrder = "0x00", hashWhere = "" }
            );

            //routes.MapRoute(
            //    name: "str1",
            //    url: "Home/Login/{returnUrl}",
            //    defaults: new { controller = "Home", action = "Login"}
            //);

        }
    }
}