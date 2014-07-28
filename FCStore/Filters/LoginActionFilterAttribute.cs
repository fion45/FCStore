using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Controllers;

namespace FCStore.Filters
{
    public class LoginActionFilterAttribute : ActionFilterAttribute
    {
        public int beforeTag;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (beforeTag <= 0)
            {
                LoginPageTrailController tmpCon = new LoginPageTrailController();
                int tmpTag = tmpCon.GetLoginPageTag(filterContext);
                filterContext.HttpContext.Session["LPTAG"] = tmpTag;
                if (tmpTag == -2)
                {
                    filterContext.HttpContext.Session["LOGINLOCK"] = tmpCon.waitSeconds;
                }
            }
        }

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);
        //}

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            if (beforeTag >= 0)
            {
                LoginPageTrailController tmpCon = new LoginPageTrailController();
                tmpCon.WriteTrail(filterContext);
            }
        }

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    base.OnResultExecuted(filterContext);
        //}
    }
}