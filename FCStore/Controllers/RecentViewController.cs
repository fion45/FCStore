using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class RecentViewController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        
        public void WriteTrail(HttpRequestBase request)
        {
            if (!request.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }
            MyUser tmpUser = request.RequestContext.HttpContext.User as MyUser;
            if (request.RequestContext.RouteData.Route != null && tmpUser != null)
            {
                object tmpObj = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["controller"];
                if (tmpObj == null || tmpObj.ToString() != "Product")
                {
                    return;
                }
                tmpObj = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["action"];
                if (tmpObj == null || tmpObj.ToString() != "Detail")
                {
                    return;
                }
            }
            else
            {
                return;
            }
            RecentView tmpRV = new RecentView();
            string tmpStr = request.Url.ToString();
            int tmpI;
            if (int.TryParse(tmpStr.Substring(tmpStr.LastIndexOf('/') + 1), out tmpI))
            {
                tmpRV.PID = tmpI;
            }
            else
            {
                return;
            }
            tmpRV.UID = tmpUser.UID;
            tmpRV.ViewDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            db.RecentViews.Add(tmpRV);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
