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


namespace WX
{
    public class WXHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string TOKEN = "";
        private static string APPID = "";
        private static string APPSECRET = "";
        private static string ACCESSTOKEN = "";

        public WXHelper(string token,string appID, string appSecret, string accessToken)
        {
            TOKEN = token;
            APPID = appID;
            APPSECRET = appSecret;
            ACCESSTOKEN = accessToken;
        }

        public string DealWith(HttpRequestBase Request)
        {
            string token = TOKEN;
            logger.Log(LogLevel.Trace, "IN WEIXIN API[" + Request.Url.ToString() + "]");
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
