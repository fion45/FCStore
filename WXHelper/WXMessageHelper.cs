using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Security;
using System.Xml;

namespace WX
{
    public static class WXMessageHelper
    {
        //返回消息
        public static string ReturnMessage(string postStr)
        {
            string responseContent = "";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(postStr)));
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
            if (MsgType != null)
            {
                switch (MsgType.InnerText)
                {
                    case "event":
                        {
                            //事件处理
                            responseContent = WXMessageHelper.EventHandle(xmldoc);
                            break;
                        }
                    case "text":
                        {
                            //接受文本消息处理
                            responseContent = WXMessageHelper.TextHandle(xmldoc);
                            break;
                        }
                    case "image":
                        {
                            //接受图片消息处理
                            responseContent = WXMessageHelper.ImageHandle(xmldoc);
                            break;
                        }
                    case "voice":
                        {
                            //接受语音消息处理
                            responseContent = WXMessageHelper.VoiceHandle(xmldoc);
                            break;
                        }
                    case "video":
                        {
                            //接受视频消息处理
                            responseContent = WXMessageHelper.VideoHandle(xmldoc);
                            break;
                        }
                    default:
                        break;
                }
            }
            return responseContent;
        }
        //事件
        public static string EventHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
            XmlNode EventKey = xmldoc.SelectSingleNode("/xml/EventKey");
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            if (Event != null)
            {
                switch(Event.InnerText.ToLower())
                {
                    case "subscribe":
                        {
                            //订阅

                            break;
                        }
                    case "unsubscribe":
                        {
                            //取消订阅
                            break;
                        }
                    case "scan":
                        {
                            //扫描二维码
                            //根据EventKey判断扫描的二维码
                            break;
                        }
                    case "location":
                        {
                            //上报地理位置
                            break;
                        }
                    case "click":
                        {
                            //点击菜单事件
                            //根据EventKey判断点击的菜单
                            break;
                        }
                    case "view":
                        {
                            //点击菜单跳转链接时的事件推送
                            //根据EventKey判断跳转链接的地址
                            break;
                        }
                }
            }
            return responseContent;
        }

        //接受文本消息
        public static string TextHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                responseContent = string.Format(ReplyFormat.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText + "\r\n<a href=\"http://www.rightgo.cn\">点击进入</a>");
            }
            return responseContent;
        }

        //接受图片消息处理
        public static string ImageHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                responseContent = string.Format(ReplyFormat.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText + "\r\n<a href=\"http://www.baidu.com\">点击进入</a>");
            }
            return responseContent;
        }

        //接受图片消息处理
        public static string VoiceHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                responseContent = string.Format(ReplyFormat.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText + "\r\n<a href=\"http://www.baidu.com\">点击进入</a>");
            }
            return responseContent;
        }

        //接受图片消息处理
        public static string VideoHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                responseContent = string.Format(ReplyFormat.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText + "\r\n<a href=\"http://www.baidu.com\">点击进入</a>");
            }
            return responseContent;
        }



        //写入日志
        public static void WriteLog(string text)
        {
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath(".") + "\\log.txt", true);
            sw.WriteLine(text);
            sw.Close();//写入
        }
    }


    //回复类型
    public static class ReplyFormat
    {
        /// <summary>
        /// 普通文本消息
        /// </summary>
        public static string Message_Text = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>";
        /// <summary>
        /// 图片消息
        /// </summary>
        public static string Message_Image = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[image]]></MsgType>
                            <Image>
                            <MediaId><![CDATA[{3}]]></MediaId>
                            </Image>
                            </xml>";
        /// <summary>
        /// 语音消息
        /// </summary>
        public static string Message_Voice = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[voice]]></MsgType>
                            <Voice>
                            <MediaId><![CDATA[{3}]]></MediaId>
                            </Voice>
                            </xml>";
        /// <summary>
        /// 视频消息
        /// </summary>
        public static string Message_Video = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[video]]></MsgType>
                            <Video>
                            <MediaId><![CDATA[{3}]]></MediaId>
                            </Video>
                            </xml>";

        /// <summary>
        /// 音乐消息
        /// </summary>
        public static string Message_Music = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[music]]></MsgType>
                            <Music>
                            <Title><![CDATA[{3}]]></Title>
                            <Description><![CDATA[{4}]]></Description>
                            <MusicUrl><![CDATA[{5}]]></MusicUrl>
                            <HQMusicUrl><![CDATA[{6}]]></HQMusicUrl>
                            <ThumbMediaId><![CDATA[{7}]]></ThumbMediaId>
                            </Music>
                            </xml> ";
        /// <summary>
        /// 图文消息
        /// </summary>
        public static string Message_News = @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[news]]></MsgType>
                            <ArticleCount>{3}</ArticleCount>
                            <Articles>
                            {4}
                            </Articles>
                            </xml> ";
        /// <summary>
        /// 图文消息项
        /// </summary>
        public static string Message_News_Item = @"<item>
                            <Title><![CDATA[{0}]]></Title> 
                            <Description><![CDATA[{1}]]></Description>
                            <PicUrl><![CDATA[{2}]]></PicUrl>
                            <Url><![CDATA[{3}]]></Url>
                            </item>";
    }
}