﻿using System;
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
                name: "zone",
                url: "Common/GetZoneList/{PID}/{CID}",
                defaults: new { controller = "Common", action = "GetZoneList"}
            );

            routes.MapRoute(
                name: "buy",
                url: "Product/buy/{id}/{count}",
                defaults: new { controller = "Product", action = "buy", id = UrlParameter.Optional, count = 1 }
            );

            routes.MapRoute(
                name: "ProductSelect",
                url: "Manager/ProductManager/{Tag}/{Par}/{BeginIndex}/{GetCount}/{OrderStr}/{WhereStr}",
                defaults: new { controller = "Manager", action = "ProductManager", Tag = "-1", Par = "null", BeginIndex = 0, GetCount = 50, OrderStr = "PID,DESC", WhereStr = "" }
            );

            routes.MapRoute(
                name: "BrandSelect",
                url: "Manager/BrandsManager/{Tag}/{Par}",
                defaults: new { controller = "Manager", action = "BrandsManager" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductDetail",
                url: "Product/Detail/{id}/{tag}",
                defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductList",
                url: "{controller}/{action}/{id}/{pIndex}/{hashOrder}/{hashWhere}",
                defaults: new { id = UrlParameter.Optional, pIndex = 1, hashOrder = "0x00", hashWhere = "" }
            );
        }
    }
}