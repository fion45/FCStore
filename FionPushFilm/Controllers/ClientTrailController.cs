using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FionPushFilm.Models;
using FionPushFilm.Common;

namespace FionPushFilm.Controllers
{
    public class ClientTrailController : Controller
    {
        private FilmDbContext db = new FilmDbContext();

        public void WriteTrail(HttpRequest request)
        {
            ClientTrail tmpCT = new ClientTrail();
            tmpCT.URL = request.Url.ToString();
            if (request.RequestContext.RouteData.Route != null)
            {
                object tmpObj = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["controller"];
                tmpCT.ControllerName = tmpObj != null ? tmpObj.ToString() : "";
                tmpObj = request.RequestContext.RouteData.Route.GetRouteData(request.RequestContext.HttpContext).Values["action"];
                tmpCT.ActionName = tmpObj != null ? tmpObj.ToString() : "";
            }
            else
            {
                tmpCT.ControllerName = "";
                tmpCT.ActionName = "";
            }
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
