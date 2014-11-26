using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Web.Security;
using System.IO;
using FCStore.Common;
using System.Collections;
using Newtonsoft.Json;

namespace FCStore.Controllers
{
    public class WeiXinController : Controller
    {
        //
        // GET: /WeiXin/

        private static string TOKEN = ConfigurationManager.AppSettings["WeixinToken"];
        private static string APPID = ConfigurationManager.AppSettings["WeixinAPPID"];
        private static string APPSECRET = ConfigurationManager.AppSettings["WeixinAPPSECRET"];
        private static string ACCESSTOKEN = "";

        public ActionResult API()
        {
            string token = TOKEN;
            string signature = Request.QueryString["signature"].ToString();
            string timestamp = Request.QueryString["timestamp"].ToString();
            string nonce = Request.QueryString["nonce"].ToString();
            string echostr = Request.QueryString["echostr"].ToString();

            if (CheckSignature(signature,timestamp,nonce,token))
            {
                if (Request.HttpMethod.ToUpper() == "POST")
                {
                    //处理微信发送的数据
                    using (Stream stream = Request.InputStream)
                    {
                        Byte[] postBytes = new Byte[stream.Length];
                        stream.Read(postBytes, 0, (Int32)stream.Length);
                        string postString = Encoding.UTF8.GetString(postBytes);
                        string responseContent = WXMessageHelper.ReturnMessage(postString);

                        Response.ContentEncoding = Encoding.UTF8;
                        Response.Write(responseContent);
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                {
                    Response.Write(echostr);
                    Response.Flush();
                    Response.End();
                }
            }
            return new EmptyResult();
        }

        public ActionResult CreateMenu()
        {

            WeiXinController.ValidateAccessToken();
            FileStream fs1 = new FileStream(Server.MapPath(".") + "\\menu.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("GBK"));
            string menu = sr.ReadToEnd();
            sr.Close();
            fs1.Close();
            string JSONStr = PubFunction.GetWebPageByPost("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + ACCESSTOKEN, menu);
            Hashtable tmpHT = (Hashtable)JsonConvert.DeserializeObject(JSONStr,typeof(Hashtable));
            if(tmpHT["errcode"].ToString() == "0")
            {
                //创建成功
            }
            return View();
        }

        public static bool ValidateAccessToken()
        {
            if(!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                //先通过获取Menu判断是否超时
                string JSONStr = PubFunction.GetWebPageByGet("https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" + ACCESSTOKEN);

            }
            RefreshAccessToken();
        }

        public static void RefreshAccessToken()
        {

            string JSONStr = PubFunction.GetWebPageByGet("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + APPID + "&secret=" + APPSECRET);

        }

        private bool CheckSignature(string signature, string timestamp, string nonce, string token)
        {
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
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
    }
}
