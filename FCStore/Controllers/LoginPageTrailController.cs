using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using FCStore.Common;
using Newtonsoft.Json;

namespace FCStore.Controllers
{
    public class LoginPageTrailController : Controller
    {
        private const int CHECKCODESHOW = 3;
        private const int LOGINLOCK = 8;
        private const int LOCKTIME = 60 * 5;        //锁住5分钟

        private FCStoreDbContext db = new FCStoreDbContext();

        public int waitSeconds = 0;

        public int GetLoginPageTag(ActionExecutingContext context)
        {
            int result = 0;
            string IP = context.HttpContext.Request.UserHostAddress;
            LoginPageTrail tmpLPT = db.LoginPageTrails.FirstOrDefault(r => r.ClientIP.CompareTo(IP) == 0);
            if(tmpLPT != null)
            {
                if (tmpLPT.ErrorCount > LOGINLOCK)
                {
                    result = -2;
                    DateTime tmpDT = DateTime.Parse(tmpLPT.LogDate);
                    tmpDT = tmpDT.AddSeconds(LOCKTIME);
                    TimeSpan tmpTS = tmpDT - DateTime.Now;
                    if(tmpTS.TotalSeconds < 0)
                    {
                        tmpLPT.ErrorCount = 0;
                        db.SaveChanges();
                        result = 0;
                        waitSeconds = 0;
                    }
                    else
                    { 
                        waitSeconds = (int)tmpTS.TotalSeconds;
                    }
                }
                else if (tmpLPT.ErrorCount > CHECKCODESHOW)
                {
                    result = -1;
                }
            }
            return result;
        }

        public int WriteTrail(ResultExecutingContext context)
        {
            int result = 0;
            string tmpStr = ((System.Web.Mvc.ContentResult)(context.Result)).Content;
            Hashtable tc = (Hashtable)Newtonsoft.Json.JsonConvert.DeserializeObject(tmpStr, typeof(Hashtable));

            string IP = context.HttpContext.Request.UserHostAddress;
            LoginPageTrail tmpLPT = db.LoginPageTrails.FirstOrDefault(r => r.ClientIP.CompareTo(IP) == 0);
            if (int.Parse(tc["errCode"].ToString()) == 0)
            {
                //登陆成功
                if(tmpLPT != null)
                {
                    tmpLPT.ErrorCount = 0;
                    tmpLPT.LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else
            {
                //登陆失败
                if (tmpLPT == null)
                {
                    tmpLPT = new LoginPageTrail();
                    tmpLPT.ClientIP = context.HttpContext.Request.UserHostAddress;
                    tmpLPT.ErrorCount = 0;
                    db.LoginPageTrails.Add(tmpLPT);
                }
                ++tmpLPT.ErrorCount;
                tmpLPT.LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (tmpLPT != null)
            {
                if (tmpLPT.ErrorCount == CHECKCODESHOW + 1)
                {
                    ((System.Web.Mvc.ContentResult)(context.Result)).Content = ((System.Web.Mvc.ContentResult)(context.Result)).Content.Replace(",\"custom\":0,", ",\"custom\":-1,");
                }
                db.SaveChanges();
                if (tmpLPT.ErrorCount > LOGINLOCK)
                {
                    ((System.Web.Mvc.ContentResult)(context.Result)).Content = ((System.Web.Mvc.ContentResult)(context.Result)).Content.Replace(",\"custom\":0,", ",\"custom\":-2,");
                    result = -2;
                }
                else if (tmpLPT.ErrorCount > CHECKCODESHOW)
                {
                    result = -1;
                }
            }
            return result;
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
