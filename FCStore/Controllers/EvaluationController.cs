using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Security;
using FCStore.Common;
using System.Text.RegularExpressions;

namespace FCStore.Controllers
{
    public class EvaluationController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult getEvaluationByPID(int ID)
        {
            //获得销售记录
            int[] NCStatus = new int[] { (int)Order.EOrderStatus.OS_Init };
            List<OrderPacket> tmpOPLST = db.OrderPackets.Where(r => r.PID == ID && !NCStatus.Contains(r.Order.Status)).ToList();
            List<int> tmpOIDArr = (from opl in tmpOPLST
                                   select opl.Order.OID).Distinct().ToList();
            //只取前20条
            List<Evaluation> tmpELST = db.Evaluations.Where(r => tmpOIDArr.Contains(r.OID)).Take(20).ToList();
            if (Request.IsAjaxRequest())
            {
                string customStr = "";
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    MyUser tmpUser = HttpContext.User as MyUser;
                    //查找该用户是否有购买该产品并未提交评价的
                    OrderPacket tmpOP = db.OrderPackets.OrderByDescending(r => r.Order.OrderDate).FirstOrDefault(r => r.PID == ID && r.Order.UID == tmpUser.UID);
                    string tmpME = "null";
                    if (tmpOP != null)
                    {
                        //已购买
                        Evaluation tmpEva = db.Evaluations.FirstOrDefault(r => r.PID == ID && r.OID == tmpOP.OID);
                        if (tmpEva != null)
                        {
                            //已评价
                            EvaluationVM tmpEVModel = new EvaluationVM(tmpEva);
                            tmpME = Newtonsoft.Json.JsonConvert.SerializeObject(tmpEVModel);
                        }
                    }
                    customStr = string.Format("{{\"buyedOID\":{0},\"myEvaluation\":{1}}}", tmpOP == null ? -1 : tmpOP.OID, tmpME);
                }
                List<EvaluationVM> tmpEVM = new List<EvaluationVM>(tmpELST.Count);
                foreach(Evaluation tmpE in tmpELST)
                {
                    tmpEVM.Add(new EvaluationVM(tmpE));
                }
                List<ShamOrderData> shamLST = db.ShamOrderDatas.Where(r => r.ProductID == ID).Take(20).ToList();
                foreach (ShamOrderData sham in shamLST)
                {
                    tmpEVM.Add(new EvaluationVM(sham));
                }
                tmpEVM.Sort((a, b) => b.DataTime.CompareTo(a.DataTime));
                string jsonStr = PubFunction.BuildResult(tmpEVM, customStr);
                return Content(jsonStr);
            }
            else
            {
                return View(tmpELST);
            }
        }

        public ActionResult submitEvaluation(Evaluation evaluation)
        {

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                MyUser user = HttpContext.User as MyUser;
                if (user != null)
                    evaluation.UID = user.UID;
                evaluation.DataTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                db.Evaluations.Add(evaluation);
                db.SaveChanges();
                evaluation.User = db.Users.FirstOrDefault(r => r.UID == evaluation.UID);
                evaluation.Order = db.Orders.FirstOrDefault(r => r.OID == evaluation.OID);
                EvaluationVM tmpEVM = new EvaluationVM(evaluation);
                if (Request.IsAjaxRequest())
                {
                    string jsonStr = PubFunction.BuildResult(tmpEVM);
                    return Content(jsonStr);
                }
                else
                {
                    return View(tmpEVM);
                }
            }
            else
            {
                string jsonStr = PubFunction.BuildResult("Err");
                return Content(jsonStr);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}