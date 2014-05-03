using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;

namespace FCStore.Controllers
{
    public class ProductController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Detail(int ID)
        {
            Product tmpProduct = db.Products.First(r => r.PID == ID);
            return View(tmpProduct);
        }

        public ActionResult ListByCategory(int ID, int PIndex)
        {
            HashSet<int> CIDSet = new HashSet<int>();
            List<Category> CatArr = db.Categorys.ToList();
            Category tmpCat = CatArr.Find(r => r.CID == ID);
            if(tmpCat == null)
            {

            }
            CIDSet.Add(tmpCat.CID);
            List<int> CIDArr = (from cat in CatArr
                        where cat.ParCID == tmpCat.CID
                        select cat.CID).ToList();
            int tmpCount = CIDArr.Count;
            for (int i = 0; i < tmpCount; i++)
            {
                CIDArr.AddRange((from cat in CatArr
                                 where cat.ParCID == CIDArr[i]
                                 select cat.CID).ToList());
            }
            foreach (int tmpCID in CIDArr)
            {
                CIDSet.Add(tmpCID);
            }
            List<Product> productArr = db.Products.Where(r => CIDSet.Contains(r.CID)).ToList();
            List<Brand> brandArr = (from pro in productArr
                                    select pro.Brand).Distinct().ToList();
            if (productArr.Count > 30)
                productArr.RemoveRange(30, productArr.Count - 30);
            ProductListVM tmpVM = new ProductListVM();
            tmpVM.Products = productArr;
            tmpVM.Brands = brandArr;
            tmpVM.PageCount = 10;
            tmpVM.PageIndex = PIndex;
            return View(tmpVM);
        }

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