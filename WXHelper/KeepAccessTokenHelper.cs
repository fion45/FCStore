using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using NLog;

namespace WX
{
    public class KeepAccessTokenHelper
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Thread mKeepThread = null;
        private Mutex mSync = new Mutex();
        private bool mRunTag = false;
        private ManualResetEvent mStopTrigger = new ManualResetEvent(false);
        private ManualResetEvent mGetTrigger = new ManualResetEvent(false);
        private string mAccessToken = null;

        private volatile static KeepAccessTokenHelper mInstance = null;
        private static readonly object lockHelper = new object();

        public static string APPID
        {
            get;
            set;
        }

        public static string APPSECRET
        {
            get;
            set;
        }

        private KeepAccessTokenHelper()
        {
            
        }

        public static KeepAccessTokenHelper Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockHelper)
                    {
                        if (mInstance == null)
                            mInstance = new KeepAccessTokenHelper();
                    }
                }
                return mInstance;
            }
        }

        ~KeepAccessTokenHelper()
        {
            Stop();
        }

        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(mAccessToken))
                {
                    mGetTrigger.Reset();
                    Start();
                }
                mGetTrigger.WaitOne();
                return mAccessToken;
            }
        }

        public void Start()
        {
            mSync.WaitOne();
            if(!mRunTag)
            {
                mRunTag = true;
                mStopTrigger.Reset();

                mKeepThread = new Thread(Running);
                try
                { 
                    mKeepThread.Start();
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Trace, ex.Message);
                }
            }
            mSync.ReleaseMutex();
        }

        public void Stop()
        {
            mSync.WaitOne();
            if (mRunTag)
            {
                mRunTag = false;
                mStopTrigger.Set();

                if (mKeepThread != null)
                {
                    mKeepThread.Join(1000);
                    mKeepThread = null;
                }
            }
            mSync.ReleaseMutex();
        }

        private void Running()
        {
            int WaitMS = 10;
            int RetryCount = 0;
            while(mRunTag)
            {
                mGetTrigger.Reset();
                string tmpURL = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + APPID + "&secret=" + APPSECRET;
                logger.Log(LogLevel.Trace, "URL:" + tmpURL);
                string JSONStr = WXAPI.GetWebPageByGet(tmpURL);
                logger.Log(LogLevel.Trace, "JSON Str:" + JSONStr);
                Hashtable tmpHT = (Hashtable)JsonConvert.DeserializeObject(JSONStr, typeof(Hashtable));
                if (tmpHT.ContainsKey("access_token"))
                {
                    //获取成功
                    RetryCount = 0;
                    mAccessToken = tmpHT["access_token"].ToString();
                    int tmpS = 0;
                    if (int.TryParse(tmpHT["expires_in"].ToString(), out tmpS))
                    {
                        WaitMS = 1000 * tmpS;
                    }
                    else
                    {
                        WaitMS = 10;
                    }
                    mGetTrigger.Set();
                    logger.Log(LogLevel.Trace, "Get AccessToken Success! AccessToken:" + mAccessToken + "|Expires_in:" + tmpS.ToString());
                }
                else
                {
                    logger.Log(LogLevel.Trace, "Get AccessToken Error! Errcode:" + tmpHT["errcode"].ToString() + "|Errmsg:" + tmpHT["errmsg"].ToString());
                    ++RetryCount;
                    if(RetryCount >= 5)
                    {
                        //获取失败
                        break;
                    }
                }
                if (mStopTrigger.WaitOne(WaitMS))
                    break;
            }
            mGetTrigger.Set();
            mStopTrigger.Set();
            mRunTag = false;
        }

    }
}
