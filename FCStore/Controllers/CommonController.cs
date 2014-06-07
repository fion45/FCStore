using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using FCStore.Models;
using FCStore.Common;
using System.Text.RegularExpressions;

namespace FCStore.Controllers
{
    public class CommonController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Common/

        public ActionResult GetProvinceArr()
        {
            StringBuilder jsonStr = new StringBuilder("\"ProvinceArr\":[");
            List<Province> ProvinceArr = db.Province.ToList();
            foreach (Province province in ProvinceArr)
            {
                jsonStr.AppendFormat("{{\"PID\":\"{0}\",\"PName\":\"{1}\"}},", province.ProvinceID, province.Name);
            }
            jsonStr.Remove(jsonStr.Length - 1, 1);
            jsonStr.Append("]");
            if (Request.IsAjaxRequest())
            {
                string resultStr = PubFunction.BuildResult("OK", jsonStr.ToString());
                return Content(resultStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult GetZoneList(int PID,int CID = -1)
        {
            StringBuilder jsonStr = new StringBuilder("\"CityArr\":[");
            Province tmpPro = db.Province.FirstOrDefault(r => r.ProvinceID == PID);
            if(tmpPro == null)
            {
                tmpPro = db.Province.FirstOrDefault();
            }
            foreach(City city in tmpPro.CityArr)
            {
                jsonStr.AppendFormat("{{\"CID\":\"{0}\",\"CName\":\"{1}\"}},", city.CityID, city.Name);
            }
            jsonStr.Remove(jsonStr.Length - 1, 1);
            jsonStr.Append("],\"TownArr\":[");
            City tmpCity = tmpPro.CityArr.FirstOrDefault(r=>r.CityID == CID);
            if(tmpCity == null)
            {
                tmpCity = tmpPro.CityArr[0];
            }
            foreach (Town town in tmpCity.TownArr)
            {
                jsonStr.AppendFormat("{{\"TID\":\"{0}\",\"TName\":\"{1}\"}},", town.TownID, town.Name);
            }
            jsonStr.Remove(jsonStr.Length - 1, 1);
            jsonStr.Append("]");
            if (Request.IsAjaxRequest())
            {
                string resultStr = PubFunction.BuildResult("OK", jsonStr.ToString());
                return Content(resultStr);
            }
            else
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
