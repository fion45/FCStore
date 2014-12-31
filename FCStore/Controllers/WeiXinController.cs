using System;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using NLog;
using WX;

namespace FCStore.Controllers
{
    public class WeiXinController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string TOKEN = ConfigurationManager.AppSettings["WeixinToken"];
        private static string APPID = ConfigurationManager.AppSettings["WeixinAPPID"];
        private static string APPSECRET = ConfigurationManager.AppSettings["WeixinAPPSECRET"];
        private static string ACCESSTOKEN = "";
        public ActionResult API()
        {
            KeepAccessTokenHelper.APPID = APPID;
            KeepAccessTokenHelper.APPSECRET = APPSECRET;
            ACCESSTOKEN = KeepAccessTokenHelper.Instance.AccessToken;
            WXHelper helper = new WXHelper(TOKEN, APPID, APPSECRET, ACCESSTOKEN);
            string responseContent = helper.DealWith(Request);
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(responseContent);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }
    }
}
