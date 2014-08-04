using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.FilterAttribute;
using FCStore.Models;
using System.Text;

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
            typeDic["ImgPath"] = new ManagerVM<BannerItem>.TableColumn.Config();
            typeDic["ImgPath"].specialType = ManagerVM<BannerItem>.TableColumn.TCType.Img;
            ManagerVM<BannerItem> tmpVM = new ManagerVM<BannerItem>(db.BannerItems.ToList(), typeDic);
            return View(tmpVM);
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

    public static class TabelItemRender<T>
    {
        public static string GetItemHTML(List<ManagerVM<T>.TableColumn> tcArr, List<ManagerVM<T>.TableItem> lst)
        {
            StringBuilder tmpSB = new StringBuilder("");
            int index = 0;
            foreach (ManagerVM<T>.TableItem ti in lst)
            {
                ManagerVM<T>.TableColumn column = tcArr[index];
                switch(column.Type)
                {
                    case ManagerVM<T>.TableColumn.TCType.Text:
                        {
                            tmpSB.Append("<td class=\'textTB\' >" + ti.Description + "</td>");
                            break;
                        }
                    case ManagerVM<T>.TableColumn.TCType.Selection:
                        {
                            tmpSB.Append("<td class=\'selTB\' >" + ti.Description + "</td>");
                            break;
                        }
                    case ManagerVM<T>.TableColumn.TCType.Img:
                        {
                            tmpSB.Append("<td class=\'imgTB\' ><img src=" + ti.Description + " /></td>");
                            break;
                        }
                    case ManagerVM<T>.TableColumn.TCType.BoolTag:
                        {
                            tmpSB.Append("<td class=\'boolTB\' >" + ti.Description + "</td>");
                            break;
                        }
                }
                ++index;
            }
            return tmpSB.ToString();
        }
    }
}
