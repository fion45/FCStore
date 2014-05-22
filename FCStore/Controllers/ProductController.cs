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
    public class ProductController : Controller
    {
        static public string ORDERCOOKIERGX = "^(?<ORDERID>\\d+?),((?<COUNT>[^,]+?),(?<TITLE>[^,]+?),(?<IMG>[^,]+?),)*$";

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

        public ActionResult ListByCategory(int ID, int PIndex = 1, string hashOrder = "0x00", string hashWhere = "")
        {
            List<int> brandWhere = GetBrandWhere(hashWhere);
            List<OrderObj> orderObjList = GetOrderObj(hashOrder);
            int PCount = 40;
            int.TryParse(ConfigurationManager.AppSettings["PCPerPage"], out PCount);

            List<int> CIDList = new List<int>();
            List<Category> CatArr = db.Categorys.ToList();
            Category tmpCat = CatArr.Find(r => r.CID == ID);
            if(tmpCat == null || tmpCat.ParCID == tmpCat.CID)
            {
                //页面不存在
                return Redirect("/Home/Index");
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
                        //只有一个排序条件
                        ViewBag.Order = oo.Type;
                        ViewBag.OrderType = oo.AscTag;
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
                {
                    ViewBag.chkBrands = brandWhere;
                    productEnum = productEnum.Where(r => brandWhere.Contains(r.BID));
                }
                List<Product> productArr = productEnum.Skip((PIndex - 1) * PCount).Take(PCount).ToList();
                List<int> BIDList = (from product in db.Products
                                     where CIDList.Contains(product.CID)
                                    select product.BID).Distinct().ToList();
                int PageCount = productEnum.Count();
                PageCount = (int)Math.Ceiling((float)PageCount / PCount);
                //获得品牌列表
                //List<Brand> brandArr = (from pro in productArr
                //                        select pro.Brand).Distinct().ToList();        //会导致每个Product都从数据库中读取向对应的brand

                List<Brand> brandArr = (from brand in db.Brands
                                        where BIDList.Contains(brand.BID)
                                        select brand).ToList();
                ProductListVM tmpVM = new ProductListVM();
                //tmpVM.Products = new Product[tmpVM.pArrCount];
                tmpVM.Products = productArr;
                tmpVM.Brands = brandArr;
                tmpVM.Category = tmpCat;
                tmpVM.PageCount = PageCount;
                tmpVM.PageIndex = PIndex;

                if (Request.IsAjaxRequest())
                {
                    //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(tmpVM);
                    string jsonStr = PubFunction.BuildResult(tmpVM);
                    return Content(jsonStr);
                }
                else
                {
                    return View(tmpVM);
                }
            }
        }


        public ActionResult ListByBrand(int ID, int PIndex, string hashOrder)
        {
            List<OrderObj> orderObjList = GetOrderObj(hashOrder);
            int PCount = 40;
            int.TryParse(ConfigurationManager.AppSettings["PCPerPage"], out PCount);

            List<int> CIDList = new List<int>();
            Brand tmpBrand = (from brand in db.Brands
                             where brand.BID == ID
                             select brand).First();
            if (tmpBrand == null)
            {
                //页面不存在
                return Redirect("aaa");
            }
            else
            {
                //获得产品列表
                var productEnum = from product in db.Products
                                            where product.BID == ID
                                            select product;
                if(orderObjList != null)
                {
                    foreach(OrderObj oo in orderObjList)
                    {
                        //只有一个排序条件
                        ViewBag.Order = oo.Type;
                        ViewBag.OrderType = oo.AscTag;
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
                List<Product> productArr = productEnum.Skip((PIndex - 1) * PCount).Take(PCount).ToList();
                int PageCount = (from product in db.Products
                                 where product.BID == ID
                                 select product).Count();
                PageCount = (int)Math.Ceiling((float)PageCount / PCount);
                ProductListVM tmpVM = new ProductListVM();
                //tmpVM.Products = new Product[tmpVM.pArrCount];
                tmpVM.Products = productArr;
                tmpVM.Brand = tmpBrand;
                tmpVM.Brands = null;
                tmpVM.Category = null;
                tmpVM.PageCount = PageCount;
                tmpVM.PageIndex = PIndex;

                if (Request.IsAjaxRequest())
                {
                    //string jsonStr = JsonConvert.SerializeObject(tmpVM);
                    string jsonStr = PubFunction.BuildResult(tmpVM);
                    return Content(jsonStr);
                }
                else
                {
                    return View(tmpVM);
                }
            }
        }

        public ActionResult Buy(int id,int count)
        {
            Product product = db.Products.First(r => r.PID == id);
            OrderPacket packet = new OrderPacket();
            packet.PID = id;
            packet.Product = product;
            packet.Univalence = product.Price;
            packet.Discount = product.Discount;
            packet.Count = count;
            Order order = null;
            string tmpStr = "";
            //添加到cookie里
            bool hasCookie = Request.Cookies.AllKeys.Contains("Order");
            HttpCookie cookie = null;
            if (hasCookie)
            {
                cookie = Request.Cookies["Order"];
                tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(ORDERCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(tmpStr);
                if(!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    Group gi = tmpMatch.Groups["ORDERID"];
                    int OrderID = int.Parse(gi.Value);
                    order = db.Orders.First(r => r.OID == OrderID);
                    if (order.Packets == null)
                    {
                        order.Packets = new List<OrderPacket>();
                    }
                    //添加到数据库
                    order.Packets.Add(packet);
                    db.OrderPackets.Add(packet);
                    db.SaveChanges();
                    tmpStr += count.ToString() + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
                }
                else
                {
                    hasCookie = false;
                }
            }
            if (!hasCookie)
            {
                cookie = new HttpCookie("Order");
                cookie.Expires = DateTime.Now.AddMonths(1);
                order = new Order();
                order.Packets = new List<OrderPacket>();
                order.UID = 1;
                order.Postage = 0;
                order.Subscription = 0;
                order.Status = (int)Order.EOrderStatus.OS_Init;
                order.SendType = (int)Order.ESendType.ST_Direct;
                order.PayType = (int)Order.EPayType.PT_Alipay;
                order.OrderDate = null;
                order.CompleteDate = null;
                order.Packets.Add(packet);
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    //已登录
                    MyUser tmpUser = HttpContext.User as MyUser;
                    if (tmpUser != null)
                    {
                        //登陆用户
                        order.UID = tmpUser.UID;
                        order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    }
                }
                //添加到数据库
                db.Orders.Add(order);
                db.OrderPackets.Add(packet);
                db.SaveChanges();
                tmpStr = order.OID.ToString() + "," + count.ToString() + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
            }
            cookie.Value = Server.UrlEncode(tmpStr);
            Response.Cookies.Add(cookie);
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