using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Configuration;

namespace FCStore.Controllers
{
    public class ProductController : Controller
    {
        public struct OrderObj
        {
            public bool AscTag;
            public int Type;
        }

        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Detail(int ID)
        {
            Product tmpProduct = db.Products.First(r => r.PID == ID);
            return View(tmpProduct);
        }

        public List<int> GetBrandWhere(string hashWhere)
        {
            List<int> result = null;
            try
            {
                string[] strArr = hashWhere.Split(new string[] { "0x" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArr.Length > 0)
                {
                    result = new List<int>();
                    foreach (string tmpStr in strArr)
                    {
                        result.Add(int.Parse(tmpStr));
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public List<OrderObj> GetOrderObj(string hashOrder)
        {
            List<OrderObj> result = null;
            try
            {
                string[] strArr = hashOrder.Split(new string[] {"0x"},StringSplitOptions.RemoveEmptyEntries);
                if(strArr.Length > 0)
                {
                    result = new List<OrderObj>();
                    foreach(string tmpStr in strArr)
                    {
                        OrderObj obj;
                        obj.AscTag = tmpStr[0] != '0';
                        obj.Type = int.Parse(tmpStr.Substring(1));
                        result.Add(obj);
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            return result;
        }

        public ActionResult ListByCategory(int ID, int PIndex, string hashWhere, string hashOrder)
        {
            List<int> brandWhere = GetBrandWhere(hashWhere);
            List<OrderObj> orderObjList = GetOrderObj(hashOrder);
            int PCount = 40;
            int.TryParse(ConfigurationManager.AppSettings["PCPerPage"], out PCount);

            List<int> CIDList = new List<int>();
            List<Category> CatArr = db.Categorys.ToList();
            Category tmpCat = CatArr.Find(r => r.CID == ID);
            if(tmpCat == null)
            {
                //页面不存在
                return Redirect("aaa");
            }
            else
            {
                //获得其子类的CID
                CIDList.Add(tmpCat.CID);
                List<int> CIDArr = (from cat in CatArr
                                    where cat.ParCID == tmpCat.CID
                                    select cat.CID).ToList();
                int tmpCount = CIDArr.Count;
                CIDList.AddRange(CIDArr);
                for (int i = 0; i < tmpCount; i++)
                {
                    CIDList.AddRange((from cat in CatArr
                                     where cat.ParCID == CIDArr[i]
                                     select cat.CID).ToList());
                }
                //获得产品列表
                var productEnum = from product in db.Products
                                            where CIDList.Contains(product.CID)
                                            select product;
                if(orderObjList != null)
                {
                    foreach(OrderObj oo in orderObjList)
                    {
                        switch(oo.Type)
                        {
                            case 0:
                                if(oo.AscTag)
                                    productEnum = productEnum.OrderBy(r=>r.Date);
                                else
                                    productEnum = productEnum.OrderByDescending(r=>r.Date);
                                break;
                            case 1:
                                if(oo.AscTag)
                                    productEnum = productEnum.OrderBy(r=>r.Sale);
                                else
                                    productEnum = productEnum.OrderByDescending(r=>r.Sale);
                                break;
                            case 2:
                                if(oo.AscTag)
                                    productEnum = productEnum.OrderBy(r=>r.Price);
                                else
                                    productEnum = productEnum.OrderByDescending(r=>r.Price);
                                break;
                            default :
                                if(oo.AscTag)
                                    productEnum = productEnum.OrderBy(r=>r.PVCount);
                                else
                                    productEnum = productEnum.OrderByDescending(r=>r.PVCount);
                                break;
                        }
                    }
                }
                if (brandWhere != null)
                    productEnum = productEnum.Where(r => brandWhere.Contains(r.BID));
                List<Product> productArr = productEnum.Skip(1).Take(PCount).ToList();
                List<int> BIDList = (from product in db.Products
                                     where CIDList.Contains(product.CID)
                                    select product.BID).Distinct().ToList();
                int PageCount = (from product in db.Products
                                 where CIDList.Contains(product.CID)
                                 select product).Count();
                PageCount = (int)Math.Ceiling((float)PageCount / PCount);
                //获得品牌列表
                //List<Brand> brandArr = (from pro in productArr
                //                        select pro.Brand).Distinct().ToList();        //会导致每个Product都从数据库中读取向对应的brand

                List<Brand> brandArr = (from brand in db.Brands
                                        where BIDList.Contains(brand.BID)
                                        select brand).Take(16).ToList();

                if (Request.IsAjaxRequest())
                {
                    return Json(productArr);
                }
                else
                {
                    ProductListVM tmpVM = new ProductListVM();
                    tmpVM.Products = productArr;
                    tmpVM.Brands = brandArr;
                    tmpVM.Category = tmpCat;
                    tmpVM.PageCount = PageCount;
                    tmpVM.PageIndex = PIndex;
                    return View(tmpVM);
                }
            }
        }

        //public ActionResult ListByCategory(int ID, int PIndex)
        //{

        //}

        //
        // GET: /Product/

        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Brand);
            return View(products.ToList());
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            ViewBag.CID = new SelectList(db.Categorys, "CID", "NameStr");
            ViewBag.BID = new SelectList(db.Brands, "BID", "NameStr");
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CID = new SelectList(db.Categorys, "CID", "NameStr", product.CID);
            ViewBag.BID = new SelectList(db.Brands, "BID", "NameStr", product.BID);
            return View(product);
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CID = new SelectList(db.Categorys, "CID", "NameStr", product.CID);
            ViewBag.BID = new SelectList(db.Brands, "BID", "NameStr", product.BID);
            return View(product);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CID = new SelectList(db.Categorys, "CID", "NameStr", product.CID);
            ViewBag.BID = new SelectList(db.Brands, "BID", "NameStr", product.BID);
            return View(product);
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // POST: /Product/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}