using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Web.Security;
using System.Text;
using FCStore.Common;
using System.Text.RegularExpressions;

namespace FCStore.FilterAttribute
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        private static List<Role> mRoles;
        private static object mInitializerLock = new object();
        private static bool mIsInitialized;
        private static string mAllPermission;

        private string mControllerName;
        private string mActionName;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            LazyInitializer.EnsureInitialized(ref mRoles, ref mIsInitialized, ref mInitializerLock, () =>
                {
                    mRoles = new List<Role>();
                    FCStoreDbContext db = new FCStoreDbContext();
                    mRoles = db.Roles.ToList();
                    StringBuilder tmpSB = new StringBuilder();
                    tmpSB.Append(',');
                    HashSet<string> tmpSet = new HashSet<string>();
                    foreach(Role role in mRoles)
                    {
                        string[] tmpStrArr = role.Description.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
                        foreach(string PStr in tmpStrArr)
                        {
                            if(!tmpSet.Contains(PStr))
                            {
                                tmpSB.Append(PStr + ",");
                                tmpSet.Add(PStr);
                            }
                        }
                    }
                    mAllPermission = tmpSB.ToString();
                    return mRoles;
                });
            mControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            mActionName = filterContext.ActionDescriptor.ActionName;
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpCookie authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if  (authCookie ==  null  || authCookie.Value ==  "" )
            {
                //游客

                if (mAllPermission.IndexOf("," + mControllerName + ",") < 0 || mAllPermission.IndexOf("," + mControllerName + "." + mActionName + ",") < 0)
                {
                    httpContext.Response.StatusCode = 401;//无权限状态码
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //登陆用户
                FormsAuthenticationTicket authTicket;
                try
                {
                    //对当前的cookie进行解密   
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    Regex rgx = new Regex("<RIDARR>(?.+)</RIDARR><RNARR>(?.+)</RNARR><PERMISSION>(?.+)</PERMISSION>");
                    Match tmpMatch = rgx.Match(authTicket.UserData);
                    if(!string.IsNullOrEmpty(tmpMatch.Value)) {
                        string RIDArr = tmpMatch.Groups[0].Value;
                        string RNameArr = tmpMatch.Groups[1].Value;
                        string RPerArr = tmpMatch.Groups[2].Value;
                        httpContext.User = new MyUser(authTicket.Name, RIDArr, RNameArr, RPerArr);
                        if (RPerArr.IndexOf(",ALL,") < 0 && RPerArr.IndexOf("," + mControllerName + ",") < 0 || RPerArr.IndexOf("," + mControllerName + "." + mActionName + ",") < 0)
                        {
                            httpContext.Response.StatusCode = 401;//无权限状态码
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }  
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if(filterContext.HttpContext.Response.StatusCode == 401)
            {
                if(filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                        {
                            Data = new {IsSuccess = false, Message = "不好意思,登录超时,请重新登录再操作!"},
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    return;
                }
                else
                {
                    string path = HttpUtility.HtmlEncode(filterContext.HttpContext.Request.Url.AbsolutePath);
                    string strUrl = "/Home/Login/{0}";
                    filterContext.HttpContext.Response.Redirect(string.Format(strUrl, HttpUtility.UrlEncode(path)), true);
                }
            }
        }
    }
}