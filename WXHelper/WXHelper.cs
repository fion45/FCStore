using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections;
using NLog;
using System.Xml;


namespace WX
{
    public enum EMenuType
    {
        MT_ROOT,
        MT_WELCOME,
        MT_MENU,
        MT_AUTOREPLY,
        MT_UNKNOW
    }

    public class MenuItem
    {
        public int CIndex;
        public string Name;
        public EMenuType Type;
        public MenuItem Parent = null;
        public List<MenuItem> ChildrenLST = new List<MenuItem>();
    }

    public class WXHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string TOKEN = "";
        private static string APPID = "";
        private static string APPSECRET = "";
        private static string ACCESSTOKEN = "";

        private MenuItem mRoot;

        public WXHelper(string token,string appID, string appSecret, string accessToken)
        {
            TOKEN = token;
            APPID = appID;
            APPSECRET = appSecret;
            ACCESSTOKEN = accessToken;
            WXMessageHelper.WelComeDG WCDG = new WXMessageHelper.WelComeDG(WelComeFun);
            WXMessageHelper.MenuDG MDG = new WXMessageHelper.MenuDG(MenuFun);
            WXMessageHelper.AutoReplyDG ARDG = new WXMessageHelper.AutoReplyDG(AutoReplyFun);
            WXMessageHelper.SetDelegate(WCDG, MDG, ARDG);
        }

        public string DealWith(HttpRequestBase Request,MenuItem Root)
        {
            mRoot = Root;
            string token = TOKEN;
            string signature = (Request.QueryString.AllKeys.Count(r => r == "signature") > 0) ? Request.QueryString["signature"].ToString() : "";
            string timestamp = (Request.QueryString.AllKeys.Count(r => r == "timestamp") > 0) ? Request.QueryString["timestamp"].ToString() : "";
            string nonce = (Request.QueryString.AllKeys.Count(r => r == "nonce") > 0) ? Request.QueryString["nonce"].ToString() : "";
            string echostr = (Request.QueryString.AllKeys.Count(r => r == "echostr") > 0) ? Request.QueryString["echostr"].ToString() : "";
            if (CheckSignature(signature, timestamp, nonce, token))
            {
                string responseContent;
                if (Request.HttpMethod.ToUpper() == "POST")
                {
                    //处理微信发送的数据
                    using (Stream stream = Request.InputStream)
                    {
                        Byte[] postBytes = new Byte[stream.Length];
                        stream.Read(postBytes, 0, (Int32)stream.Length);
                        string postString = Encoding.UTF8.GetString(postBytes);
                        responseContent = WXMessageHelper.ReturnMessage(postString);
                    }
                }
                else
                {
                    responseContent = echostr;
                }

                logger.Log(LogLevel.Trace, "Response[" + responseContent + "]");
                return responseContent;
            }
            return "";
        }

        private string WelComeFun(XmlDocument xmldoc)
        {
            logger.Log(LogLevel.Trace, "WelCome");
            string result = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            string content = GetAutoReplyStr(EMenuType.MT_WELCOME, null, "欢迎新用户");
            result = string.Format(ReplyFormat.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    content);
            return result;
        }

        private string MenuFun(XmlDocument xmldoc)
        {
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            string[] strArr = Content.InnerText.Split(new char[] { '-' });
            return GetAutoReplyStr(EMenuType.MT_MENU, Content.InnerText);
        }

        private string AutoReplyFun(XmlDocument xmldoc)
        {
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            string[] strArr = Content.InnerText.Split(new char[] { '-' });
            return GetAutoReplyStr(EMenuType.MT_AUTOREPLY, Content.InnerText);
        }

        private bool CheckSignature(string signature, string timestamp, string nonce, string token)
        {
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetAutoReplyStr(EMenuType type, string IndexStr = null, string defaultStr = "不好意思，不明白你说什么")
        {
            string result = defaultStr;
            MenuItem MItem;
            if (string.IsNullOrEmpty(IndexStr))
                MItem = mRoot.ChildrenLST.FirstOrDefault(r => r.Type == type);
            else
            {
                int tmpI;
                string[] strArr = IndexStr.Split(new char[] { '-' });
                MenuItem tmpMenu = mRoot;
                foreach(string tmpStr in strArr)
                {
                    if(int.TryParse(tmpStr,out tmpI))
                    {
                        tmpMenu = tmpMenu.ChildrenLST.FirstOrDefault(r => r.CIndex == tmpI);
                        if (tmpMenu.ChildrenLST == null || tmpMenu.ChildrenLST.Count == 0)
                            result = tmpMenu.Name;
                        else
                        {
                            result = "";
                            foreach(MenuItem MI in tmpMenu.ChildrenLST)
                            {
                                result += MI.CIndex + ":" + MI.Name + "\r\n";
                            }
                        }
                    }
                    else
                    {
                        tmpMenu = mRoot.ChildrenLST.FirstOrDefault(r => r.Type == EMenuType.MT_UNKNOW);
                        if (tmpMenu != null)
                            result = tmpMenu.Name;
                        break;
                    }
                }
            }
            return result;
        }

        //public ActionResult CreateMenu()
        //{

        //    //WeiXinController.RefreshAccessToken();
        //    FileStream fs1 = new FileStream(Server.MapPath(".") + "\\menu.txt", FileMode.Open);
        //    StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("GBK"));
        //    string menu = sr.ReadToEnd();
        //    sr.Close();
        //    fs1.Close();
        //    string JSONStr = PubFunction.GetWebPageByPost("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + ACCESSTOKEN, menu);
        //    Hashtable tmpHT = (Hashtable)JsonConvert.DeserializeObject(JSONStr, typeof(Hashtable));
        //    if (tmpHT["errcode"].ToString() == "0")
        //    {
        //        //创建成功
        //    }
        //    return View();
        //}

        //public static bool ValidateAccessToken()
        //{
        //    if (!string.IsNullOrEmpty(ACCESSTOKEN))
        //    {
        //        //先通过获取Menu判断是否超时
        //        string JSONStr = PubFunction.GetWebPageByGet("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + ACCESSTOKEN);
        //        Hashtable tmpHT = (Hashtable)JsonConvert.DeserializeObject(JSONStr, typeof(Hashtable));
        //        if (tmpHT.ContainsKey("ip_list"))
        //        {
        //            //AccessToken有效
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
