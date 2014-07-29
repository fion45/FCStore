 using System.Web.Mvc;

namespace FCStore.Filters
{
    public class RequireHttpsAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 重写OnAuthorization方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // 如果已经是https连接则不处理,否则重定向到https连接
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                // 获取当前请求的Path
                string path = filterContext.HttpContext.Request.Path;

                // 从web.config中获取host,也可以直接从httpContext中获取
                //string host = System.Configuration.ConfigurationManager.AppSettings["HostName"];

                string host = filterContext.RequestContext.HttpContext.Request.Url.Host;

                // 从web.config中获取https的端口
                string port = System.Configuration.ConfigurationManager.AppSettings["HttpsPort"];

                // 如果端口号为空表示使用默认端口,否则将host写成host:port的形式
                if (port != null)
                {
                    host = string.Format("{0}:{1}", host, port);
                }

                // 重定向到https连接
                filterContext.HttpContext.Response.Redirect(string.Format("https://{0}{1}", host, path));
            }
        }
    }
}