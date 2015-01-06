using System;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
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
            logger.Log(LogLevel.Trace, "F0");
            logger.Log(LogLevel.Trace, APPID);
            try
            {
                logger.Log(LogLevel.Trace, KeepAccessTokenHelper.APPID);
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.Trace, ex.Message);
            }
            
            KeepAccessTokenHelper.APPID = APPID;
            logger.Log(LogLevel.Trace, "1");
            KeepAccessTokenHelper.APPSECRET = APPSECRET;
            logger.Log(LogLevel.Trace, Request.Url);
            logger.Log(LogLevel.Trace, "2");
            ACCESSTOKEN = KeepAccessTokenHelper.Instance.AccessToken;
            logger.Log(LogLevel.Trace, "3");
            WXHelper helper = new WXHelper(TOKEN, APPID, APPSECRET, ACCESSTOKEN);
            logger.Log(LogLevel.Trace, "4");
            string responseContent = helper.DealWith(Request, BuildMenu());
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(responseContent);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }
        public MenuItem BuildMenu()
        {
            MenuItem result = new MenuItem();
            result.ChildrenLST = new List<MenuItem>();
            result.Type = EMenuType.MT_ROOT;
            //TOTEST:
            MenuItem tmpItem = new MenuItem();
            tmpItem.ChildrenLST = new List<MenuItem>();
            tmpItem.CIndex = 1;
            tmpItem.Name = "欢迎新用户";
            tmpItem.Type = EMenuType.MT_WELCOME;
            tmpItem.Parent = result;
            result.ChildrenLST.Add(tmpItem);

            tmpItem = new MenuItem();
            tmpItem.ChildrenLST = new List<MenuItem>();
            tmpItem.CIndex = 100;
            tmpItem.Name = "物业服务";
            tmpItem.Type = EMenuType.MT_MENU;
            tmpItem.Parent = result;
            result.ChildrenLST.Add(tmpItem);

            MenuItem childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 1;
            childItem.Name = "公告制度";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 2;
            childItem.Name = "报修求助";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 3;
            childItem.Name = "水电安装";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 4;
            childItem.Name = "生活缴费";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 5;
            childItem.Name = "意见反馈";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            tmpItem = new MenuItem();
            tmpItem.ChildrenLST = new List<MenuItem>();
            tmpItem.CIndex = 200;
            tmpItem.Name = "生活服务";
            tmpItem.Type = EMenuType.MT_MENU;
            tmpItem.Parent = result;
            result.ChildrenLST.Add(tmpItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 1;
            childItem.Name = "餐饮服务";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 2;
            childItem.Name = "逛超市";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 3;
            childItem.Name = "水电五金";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 4;
            childItem.Name = "家居服务";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 5;
            childItem.Name = "物业租售";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            tmpItem = new MenuItem();
            tmpItem.ChildrenLST = new List<MenuItem>();
            tmpItem.CIndex = 300;
            tmpItem.Name = "服务中心";
            tmpItem.Type = EMenuType.MT_MENU;
            tmpItem.Parent = result;
            result.ChildrenLST.Add(tmpItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 1;
            childItem.Name = "社区官网";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 2;
            childItem.Name = "商家加盟";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 3;
            childItem.Name = "社区活动";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 4;
            childItem.Name = "广告推荐";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            childItem = new MenuItem();
            childItem.ChildrenLST = new List<MenuItem>();
            childItem.CIndex = 5;
            childItem.Name = "生活百事通";
            childItem.Type = EMenuType.MT_MENU;
            childItem.Parent = tmpItem;
            childItem.Content = "<a href='www.baidu.com'>www.baidu.com</a>";
            tmpItem.ChildrenLST.Add(childItem);

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            //RunTag = false;
            //Trigger.Set();
            base.Dispose(disposing);
        }
    }
}
