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
            if (authCookie == null || authCookie.Value == "" || authCookie.Value == null)
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
                try
                {
                    MyUser tmpUser = httpContext.User as MyUser;
                    if (tmpUser == null)
                    {
                        //清除状态，cookie有错误

                        httpContext.Response.StatusCode = 401;//无权限状态码
                        return false;
                    }
                    if (tmpUser.HavePermission(",ALL,") && tmpUser.HavePermissionInAction(mControllerName, mActionName))
                    {
                        return true;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 401;//无权限状态码
                        return false;
                    }
                }
                catch
                {
                    httpContext.Response.StatusCode = 401;
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
                    string path = HttpUtility.HtmlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri);
                    string strUrl = "/Home/Login/{0}";
                    filterContext.HttpContext.Response.Redirect(string.Format(strUrl, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(path))), true);
                }
            }
        }
    }
}