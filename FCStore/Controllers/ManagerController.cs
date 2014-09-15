using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.FilterAttribute;
using FCStore.Models;
using System.Text;
using FCStore.Common;
using System.IO;


namespace FCStore.Controllers
{
    public class ManagerController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Manager/

        #region BannerManager
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
            typeDic["ImgPath"].htmlStr = "data-toPath='/Uploads/Banner/'";
            typeDic["ImgPath"].width = 100;

            typeDic["Index"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["Index"].width = 50;

            typeDic["HrefPath"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["HrefPath"].width = 219;

            ManagerVM<BannerItem> tmpVM = new ManagerVM<BannerItem>(db.BannerItems.ToList(), typeDic);
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult SaveBannerManager(List<BannerItem> AddArr, List<BannerItem> EditArr, List<BannerItem> DelArr)
        {
            if (AddArr != null)
            {
                foreach (BannerItem item in AddArr)
                {
                    BannerItem tmpObj = item;
                    PubFunction.NotNullObj(ref tmpObj);
                    db.BannerItems.Add(tmpObj);
                }
            }
            if (EditArr != null)
            {
                foreach (BannerItem item in EditArr)
                {
                    BannerItem tmpObj = item;
                    //奇怪，传进来的明明是""但是在后台获取就是null，只能做转换了
                    PubFunction.NotNullObj(ref tmpObj);
                    BannerItem tmpItem = db.BannerItems.FirstOrDefault(r => r.BIID == tmpObj.BIID);
                    //tmpItem.Description = tmpObj.Description;
                    //tmpItem.HrefPath = tmpObj.HrefPath;
                    //tmpItem.ImgPath = tmpObj.ImgPath;
                    //tmpItem.Index = tmpObj.Index;
                    //tmpItem.Title = tmpObj.Title;
                    PubFunction.CopyObj(tmpObj, ref tmpItem);
                }
            }
            if (DelArr != null)
            {
                foreach (BannerItem item in DelArr)
                {
                    BannerItem tmpItem = db.BannerItems.FirstOrDefault(r => r.BIID == item.BIID);
                    if (tmpItem != null)
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
        #endregion

        #region ColumnManager
        [MyAuthorizeAttribute]
        public ActionResult ColumnManager()
        {
            Dictionary<string, ManagerVM<Column>.TableColumn.Config> typeDic = new Dictionary<string, ManagerVM<Column>.TableColumn.Config>();

            typeDic["ColumnID"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["ColumnID"].specialType = ManagerVM<Column>.TableColumn.TCType.ID;
            typeDic["ColumnID"].width = 70;

            typeDic["Describe"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["Describe"].width = 120;

            typeDic["SubDescribe"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["SubDescribe"].width = 120;

            typeDic["TopTitle"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["TopTitle"].width = 320;

            typeDic["Products"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["Products"].specialType = ManagerVM<Column>.TableColumn.TCType.Href;
            typeDic["Products"].parameter = "/Manager/ProductManager/0/{ColumnID}";
            typeDic["Products"].width = 60;

            typeDic["Brands"] = new ManagerVM<Column>.TableColumn.Config();
            typeDic["Brands"].specialType = ManagerVM<Column>.TableColumn.TCType.Href;
            typeDic["Brands"].parameter = "/Manager/BrandsSelect/0/{ColumnID}";
            typeDic["Brands"].width = 60;

            ManagerVM<Column> tmpVM = new ManagerVM<Column>(db.Columns.ToList(), typeDic);
            return View(tmpVM);
        }

        [MyAuthorizeAttribute]
        public ActionResult SaveColumnManager(List<Column> AddArr, List<Column> EditArr, List<Column> DelArr)
        {
            if (AddArr != null)
            {
                foreach (Column item in AddArr)
                {
                    Column tmpObj = item;
                    PubFunction.NotNullObj(ref tmpObj);
                    db.Columns.Add(tmpObj);
                }
            }
            if (EditArr != null)
            {
                foreach (Column item in EditArr)
                {
                    Column tmpObj = item;
                    PubFunction.NotNullObj(ref tmpObj);
                    Column tmpItem = db.Columns.FirstOrDefault(r => r.ColumnID == tmpObj.ColumnID);
                    PubFunction.CopyObj(tmpObj, ref tmpItem);
                }
            }
            if (DelArr != null)
            {
                foreach (Column item in DelArr)
                {
                    Column tmpItem = db.Columns.FirstOrDefault(r => r.ColumnID == item.ColumnID);
                    if (tmpItem != null)
                        db.Columns.Remove(tmpItem);
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
        #endregion

        [MyAuthorizeAttribute]
        public ActionResult CategoryManager()
        {
            return View();
        }

        [MyAuthorizeAttribute]
        public ActionResult ProductManager(int Tag, string Par, int BeginIndex, int GetCount, string OrderStr, string WhereStr)
        {
            int totalCount = db.Products.Count();
            StringBuilder SQLStr = new StringBuilder("SELECT TOP(");
            SQLStr.Append(GetCount);
            SQLStr.Append(") * FROM (SELECT TOP(");
            SQLStr.Append(totalCount - (BeginIndex + GetCount));
            SQLStr.Append(") * FROM Products");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                SQLStr.Append(" WHERE " + WhereStr);
            }
            string tmpOB1 = "",tmpOB2 = "";
            if (!string.IsNullOrEmpty(OrderStr))
            {
                SQLStr.Append(" ORDER BY ");
                string[] OrderArr = OrderStr.Split(new char[] { ';' });
                foreach(string itemArr in OrderArr)
                {
                    string[] itemStr = itemArr.Split(new char[]{','});
                    tmpOB1 = itemStr[0] + " " + (itemStr[1].ToUpper().CompareTo("DESC") == 0 ? "ASC" : "DESC");
                    tmpOB2 = itemStr[0] + " " + itemStr[1].ToUpper();
                }
                SQLStr.Append(tmpOB1);
            }
            SQLStr.Append(") as TEMPTB");

            if (!string.IsNullOrEmpty(OrderStr))
            {
                SQLStr.Append(" ORDER BY ");
                SQLStr.Append(tmpOB2);
            }
            List<Product> result = db.m_objcontext.ExecuteStoreQuery<Product>(SQLStr.ToString(), null).ToList();
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(result);
                return Content(jsonStr);
            }
            else
            {
                return View(result);
            }
        }

        [MyAuthorizeAttribute]
        public ActionResult BrandsSelect(int Tag, string Par)
        {
            return View(db.Brands.OrderBy(r => r.NameStr));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string tmpStr = Request.Params["toPath"].ToString();
                    string filePath = Server.MapPath("/Uploads/");
                    if(!string.IsNullOrEmpty(tmpStr))
                    {
                        filePath = Server.MapPath(tmpStr);
                    }
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                    fileData.SaveAs(filePath + saveName);

                    return Json(new { Success = true, FileName = fileName, SaveName = saveName, imgSrc = tmpStr + saveName });
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
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
                    case ManagerVM<T>.TableColumn.TCType.Href:
                        {
                            string parStr = ManagerVM<T>.ParseParameter(column.Parameter.ToString(), ti);
                            tmpStr = "<a href='" + parStr + "' >配置</a>";
                            break;
                        }
                }
                if (!string.IsNullOrEmpty(column.ClassName))
                    tmpSB.Append(" " + column.ClassName);

                tmpSB.Append("\' data-content=\'" + ti.Description + "\' >" + tmpStr + "</td>");
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
                if (!string.IsNullOrEmpty(tc.HtmlStr))
                    tmpSB.Append(tc.HtmlStr);
                tmpSB.Append(" >" + tc.Title + "</td>");
            }
            return tmpSB.ToString();
        }
    }
}
