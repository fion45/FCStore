using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Text;
using FCStore.Common;
using FCStore.Models;
using FCStore.Controllers;
using FCStore.Filters;

namespace FCStore
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //增加全局的Filter用于记录用户的Tracker
            GlobalFilters.Filters.Add(new UserTrackerLogAttribute());

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //从文件加载省份，城市，区域到数据库
            FCStoreDbContext db = new FCStoreDbContext();
            if(db.Province.Count() == 0) 
            {
                //省份不存在
                string tmpFP = ConfigurationManager.AppSettings["PCTFilePath"];
                tmpFP = Server.MapPath(tmpFP);
                FileInfo tmpFI = new FileInfo(tmpFP);
                if(tmpFI.Exists)
                {
                    FileStream tmpFS = tmpFI.OpenRead();
                    int FLen = (int)tmpFI.Length;
                    byte[] buffer = new byte[FLen];
                    //格式:<Province><PName>北京</PName><PPC>22</PPC><CityArr><City><CName>北京市</CName><CPC>44</CPC><TownArr><TName>南山区</TName><TPC>66</TCP></TownArr></City></CityArr></Province>
                    if (tmpFS.Read(buffer, 0, FLen) > 0)
                    {
                        string tmpStr = Encoding.Unicode.GetString(buffer);
                        string ProRgxStr = "<Province>\\s*?<PName>\\s*?(?<PName>\\w+?)\\s*?</PName>\\s*?<PPC>\\s*?(?<PPC>\\d+?)\\s*?</PPC>\\s*?<CityArr>\\s*?(?<PContent>.+?)\\s*?</CityArr>\\s*?</Province>";
                        Regex ProRgx = new Regex(ProRgxStr,RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        MatchCollection tmpMC = ProRgx.Matches(tmpStr);
                        foreach (Match tmpMatch in tmpMC)
                        {
                            Province tmpPro = new Province();
                            tmpPro.Name = tmpMatch.Groups["PName"].Value;
                            tmpPro.PostCode1 = tmpMatch.Groups["PPC"].Value;
                            if (tmpPro.CityArr == null)
                                tmpPro.CityArr = new List<City>();
                            string cityStr = tmpMatch.Groups["PContent"].Value;
                            string CityRgxStr = "<City>\\s*?<CName>\\s*?(?<CName>\\w+?)\\s*?</CName>\\s*?<CPC>\\s*?(?<CPC>\\d+?)\\s*?</CPC>\\s*?<TownArr>\\s*?(?<CContent>.+?)\\s*?</TownArr>\\s*?</City>";
                            Regex cityRgx = new Regex(CityRgxStr, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                            MatchCollection tmpMC1 = cityRgx.Matches(cityStr);
                            foreach (Match tmpMatch1 in tmpMC1)
                            {
                                City tmpCity = new City();
                                tmpCity.Name = tmpMatch1.Groups["CName"].Value;
                                tmpCity.PostCode2 = tmpMatch1.Groups["CPC"].Value;
                                if (tmpCity.TownArr == null)
                                    tmpCity.TownArr = new List<Town>();
                                string townStr = tmpMatch1.Groups["CContent"].Value;
                                string TownRgxStr = "<Town>\\s*?<TName>\\s*?(?<TName>\\w+?)\\s*?</TName>\\s*?<TPC>\\s*?(?<TPC>\\d+?)\\s*?</TPC>\\s*?</Town>";
                                Regex townRgx = new Regex(TownRgxStr, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                MatchCollection tmpMC2 = townRgx.Matches(townStr);
                                foreach (Match tmpMatch2 in tmpMC2)
                                {
                                    Town tmpTown = new Town();
                                    tmpTown.Name = tmpMatch2.Groups["TName"].Value;
                                    tmpTown.PostCode3 = tmpMatch2.Groups["TPC"].Value;
                                    tmpCity.TownArr.Add(tmpTown);
                                    db.Town.Add(tmpTown);
                                }
                                tmpPro.CityArr.Add(tmpCity);
                                db.City.Add(tmpCity);
                            }
                            db.Province.Add(tmpPro);
                        }
                        if(tmpMC.Count > 0)
                            db.SaveChanges();
                    }
                    tmpFS.Close();
                }
            }

            db.Dispose();

            //注册RouteDebug
            RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
        }

        protected void Application_Error(object s, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null && ex.GetType().Name == "HttpException")
            {
                HttpException exception = (HttpException)ex;
                if (exception.GetHttpCode() == 404)
                {
                    Response.StatusCode = 404;
                }
            }
            Server.ClearError();
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "" && authCookie.Value != null)
            {
                //对当前的cookie进行解密   
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                Regex rgx = new Regex("<USERID>(.+)</USERID><USERNAME>(.+)</USERNAME><RIDARR>(.+)</RIDARR><RNARR>(.+)</RNARR><PERMISSION>(.+)</PERMISSION>");
                Match tmpMatch = rgx.Match(authTicket.UserData);

                if (!string.IsNullOrEmpty(tmpMatch.Value) && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
                {
                    MyUser myUser = new MyUser(int.Parse(tmpMatch.Groups[1].Value), tmpMatch.Groups[2].Value, tmpMatch.Groups[3].Value, tmpMatch.Groups[4].Value, tmpMatch.Groups[5].Value);
                    HttpContext.Current.User = myUser;
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_LogRequest(object sender, EventArgs e)
        {
            bool enableTrail = false;
            if (System.Configuration.ConfigurationManager.AppSettings["EnableTrail"] != null)
            {
                if (bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["EnableTrail"], out enableTrail) && enableTrail)
                {
                    ClientTrailController tmpCon = new ClientTrailController();
                    tmpCon.WriteTrail(this.Request);
                }
            }
        }
    }
}