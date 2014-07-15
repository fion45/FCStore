using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class ClientTrailController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public void WriteTrail(HttpRequest request)
        {
            ClientTrail tmpCT = new ClientTrail();
            tmpCT.URL = request.Url.ToString();
            tmpCT.ControllerName = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["controller"].ToString();
            tmpCT.ActionName = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["action"].ToString();
            tmpCT.ClientIP = request.UserHostAddress;
            tmpCT.LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if(request.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                MyUser tmpUser = request.RequestContext.HttpContext.User as MyUser;
                if(tmpUser != null)
                {
                    tmpCT.UID = tmpUser.UID;
                }
            }
            db.ClientTrails.Add(tmpCT);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
