using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Controllers;
using FCStore.Common;

namespace FCStore.Filters
{
    public class ProductViewFilterAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);
        //}

        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    base.OnResultExecuting(filterContext);
        //}

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            RecentViewController tmpRVCon = new RecentViewController();
            tmpRVCon.WriteTrail(filterContext.RequestContext.HttpContext.Request);
            base.OnResultExecuted(filterContext);
        }
    }
}