using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using FCStore.Controllers;
using NLog;

namespace FCStore.Filters
{
    public class UserTrackerLogAttribute : ActionFilterAttribute 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;
            string controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = actionDescriptor.ActionName;
            DateTime timeStamp = filterContext.HttpContext.Timestamp;
            string userName = "游客";
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                userName = filterContext.HttpContext.User.Identity.Name.ToString();

            StringBuilder message = new StringBuilder();
            message.Append("UserName=");
            message.Append(userName + "|");
            message.Append("Controller=");
            message.Append(controllerName + "|");
            message.Append("Action=");
            message.Append(actionName + "|");
            message.Append("TimeStamp=");
            message.Append(timeStamp.ToString() + "|");

            logger.Log(LogLevel.Trace, message.ToString());
            base.OnActionExecuted(filterContext);
        }
    }
}