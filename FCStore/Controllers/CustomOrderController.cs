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
    public class CustomOrderController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /CustomOrder/
        public ActionResult GetEnableCountry()
        {
            if (GlobalTemp.ForeignSupplyCountryHS == null)
            {
                GlobalTemp.ForeignSupplyCountryHS = new HashSet<string>();
                List<string> ReLST = (from re in db.ReUserRoles
                                    where re.RID == (int)Role.RoleTypeID.RT_FOREIGNSUPPLIER
                                    select re.Reserve).ToList();

                foreach (string tmpStr in ReLST)
                {
                    try
                    {
                        Hashtable tmpHT = (Hashtable)JsonConvert.DeserializeObject(tmpStr, typeof(Hashtable));
                        string countryName = tmpHT["Country"].ToString();
                        GlobalTemp.ForeignSupplyCountryHS.Add(countryName);
                    }
                    catch
                    {

                    }
                }
                    
            }


            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(GlobalTemp.ForeignSupplyCountryHS);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult Save(CustomOrder customOrder)
        {
            db.CustomOrders.Add(customOrder);
            db.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult("OK");
                return Content(jsonStr);
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
