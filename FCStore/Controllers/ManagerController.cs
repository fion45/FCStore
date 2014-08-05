using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.FilterAttribute;
using FCStore.Models;
using System.Text;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class ManagerController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Manager/

        [MyAuthorizeAttribute]
        public ActionResult BannerManager()
        {
            Dictionary<string, ManagerVM<BannerItem>.TableColumn.Config> typeDic = new Dictionary<string, ManagerVM<BannerItem>.TableColumn.Config>();

            typeDic["BIID"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["BIID"].specialType = ManagerVM<BannerItem>.TableColumn.TCType.ID;
            typeDic["BIID"].width = 50;

            typeDic["Title"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["Title"].width = 120;

            typeDic["Description"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["Description"].width = 220;

            typeDic["ImgPath"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["ImgPath"].specialType = ManagerVM<BannerItem>.TableColumn.TCType.Img;
            typeDic["ImgPath"].width = 100;

            typeDic["Index"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["Index"].width = 50;

            typeDic["HrefPath"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["HrefPath"].width = 219;

            ManagerVM<BannerItem> tmpVM = new ManagerVM<BannerItem>(db.BannerItems.ToList(), typeDic);
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult SaveBannerManager(List<BannerItem> AddArr,List<BannerItem> EditArr,List<BannerItem> DelArr)
        {
            if (AddArr != null)
            {
                foreach (BannerItem bItem in AddArr)
                {
                    db.BannerItems.Add(bItem);
                }
            }
            if (EditArr != null)
            {
                foreach (BannerItem bItem in EditArr)
                {
                    BannerItem tmpItem = db.BannerItems.FirstOrDefault(r => r.BIID == bItem.BIID);
                    tmpItem.Description = bItem.Description;
                    tmpItem.HrefPath = bItem.HrefPath;
                    tmpItem.ImgPath = bItem.ImgPath;
                    tmpItem.Index = bItem.Index;
                    tmpItem.Title = bItem.Title;
                }
            }
            if (DelArr != null)
            {
                foreach (BannerItem bItem in DelArr)
                {
                    BannerItem tmpItem = db.BannerItems.FirstOrDefault(r => r.BIID == bItem.BIID);
                    if(tmpItem != null)
                        db.BannerItems.Remove(tmpItem);
                }
            }
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

        [MyAuthorizeAttribute]
        public ActionResult ColumnManager()
        {
            return View();
        }

        [MyAuthorizeAttribute]
        public ActionResult CategoryManager()
        {
            return View();
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public static class TabelRender<T>
    {
        public static string GetItemHTML(List<ManagerVM<T>.TableColumn> tcArr, List<ManagerVM<T>.TableItem> lst)
        {
            StringBuilder tmpSB = new StringBuilder("");
            int index = 0;
            foreach (ManagerVM<T>.TableItem ti in lst)
            {
                ManagerVM<T>.TableColumn column = tcArr[index];
                tmpSB.Append("<td class=\'");
                string tmpStr = ti.Description;
                tmpSB.Append(column.Type.ToString() + "TD");
                switch(column.Type)
                {
                    case ManagerVM<T>.TableColumn.TCType.Img:
                        {
                            tmpStr = "<img src=" + ti.Description + " />";
                            break;
                        }
                }
                if (!string.IsNullOrEmpty(column.ClassName))
                    tmpSB.Append(" column.ClassName");
                tmpSB.Append("\'>" + tmpStr + "</td>");
                ++index;
            }
            return tmpSB.ToString();
        }
        public static string GetColumnHTML(List<ManagerVM<T>.TableColumn> tcArr)
        {
            StringBuilder tmpSB = new StringBuilder("");
            foreach (ManagerVM<T>.TableColumn tc in tcArr)
            {
                string className = tc.Type.ToString() + "TD";
                tmpSB.Append("<td class=\'" + className + "\'");
                if (tc.Width > 0)
                    tmpSB.Append(" style=\'width:" + tc.Width + "px;\'");
                tmpSB.Append(" >" + tc.Title + "</td>");
            }
            return tmpSB.ToString();
        }
    }
}
