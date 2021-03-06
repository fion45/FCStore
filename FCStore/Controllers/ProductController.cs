﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Configuration;
using System.Web.Security;
using FCStore.Common;
using System.Text.RegularExpressions;
using FCStore.Filters;
using System.IO;


namespace FCStore.Controllers
{
    public class ProductController : Controller
    {
        static public string ORDERCOOKIERGX = "^(?<ORDERID>\\d+?),((?<PID>[^,]+?),(?<COUNT>[^,]+?),(?<TITLE>[^,]+?),(?<IMG>[^,]+?),)*$";

        public struct OrderObj
        {
            public bool AscTag;
            public int Type;
        }

        private FCStoreDbContext db = new FCStoreDbContext();

        public Category GetSerialCategory(int CID)
        {
            Category aCategory = db.Categorys.FirstOrDefault(r => r.CID == CID);
            Category result = aCategory;
            int PCID = aCategory.ParCID;
            while(PCID > 0) 
            {
                Category tmpCategory = db.Categorys.FirstOrDefault(r => r.CID == PCID);
                aCategory.Parent = tmpCategory;
                aCategory = tmpCategory;
                PCID = aCategory.ParCID;
                if (PCID == aCategory.CID)
                    break;
            }
            return result;
        }

        [ProductViewFilterAttribute]
        public ActionResult Detail(int ID, int tag = 0)
        {
            ViewBag.KeepTag = false;
            Product tmpProduct = db.Products.First(r => r.PID == ID);
            tmpProduct.Category = GetSerialCategory(tmpProduct.CID);
            bool hasCookie = Request.Cookies.AllKeys.Contains("Keeps");
            HttpCookie cookie = null;
            string tmpStr = "";
            if (hasCookie)
            {
                cookie = Request.Cookies["Keeps"];
                tmpStr = Server.UrlDecode(cookie.Value);
                Regex cookieRgx = new Regex(KeepController.KEEPCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(tmpStr);
                if (!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    Group gi = tmpMatch.Groups["PRODUCTID"];
                    foreach (Capture cap in gi.Captures)
                    {
                        if (ID == int.Parse(cap.Value))
                        {
                            ViewBag.KeepTag = true;
                            break;
                        }
                    }
                }
            }
            ViewBag.EvaluationCount = db.Evaluations.Count(r => r.Product.PID == ID);
            ViewBag.SaleCount = (from op in db.OrderPackets
                                 where op.PID == ID && op.Order.Status > 1
                                 select op.Count).ToList().Sum();
            int shamCount = db.ShamOrderDatas.Count(r => r.ProductID == ID);
            ViewBag.EvaluationCount += shamCount;
            ViewBag.SaleCount += shamCount;

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
            if(tmpCat == null || tmpCat.CID == tmpCat.ParCID)
            {
                //页面不存在
                return Redirect("/Home/Index");
            }
            else
            {
                Category parCat = tmpCat;
                //获得其父类
                while (true)
                {
                    if (parCat == null || parCat.ParCID == parCat.CID)
                        break;
                    parCat.Parent = (from cat in CatArr
                             where cat.CID == parCat.ParCID
                             select cat).FirstOrDefault();
                    parCat = parCat.Parent;
                }
                //获得其子类的CID
                CIDList.Add(tmpCat.CID);
                List<int> tmpContainLST = CIDList;
                while(true)
                {
                    int OldVal = tmpContainLST.FirstOrDefault();
                    tmpContainLST = (from cat in CatArr
                                    where tmpContainLST.Contains(cat.ParCID)
                                    select cat.CID).ToList();
                    if(tmpContainLST.Count == 1 && tmpContainLST[0] == OldVal || tmpContainLST.Count == 0)
                        break;
                    CIDList.AddRange(tmpContainLST);
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


        public ActionResult ListByBrand(int ID, int PIndex = 1, string hashOrder = "0x00")
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
                    order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                    if(order != null)
                    {
                        if (order.Packets == null)
                        {
                            order.Packets = new List<OrderPacket>();
                        }
                        //添加到数据库
                        order.Packets.Add(packet);
                        db.OrderPackets.Add(packet);
                        db.SaveChanges();
                        tmpStr += product.PID + "," + count.ToString() + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
                    }
                    else
                    {
                        order = new Order();
                        db.Orders.Add(order);
                        order.Packets = new List<OrderPacket>();
                        order.UID = null;
                        order.Postage = 0;
                        order.Subscription = 0;
                        order.Status = (int)Order.EOrderStatus.OS_Init;
                        order.SendType = (int)Order.ESendType.ST_Direct;
                        order.PayType = (int)Order.EPayType.PT_Alipay;
                        order.OrderDate = null;
                        order.CompleteDate = null;
                        order.Packets.Add(packet);
                        db.OrderPackets.Add(packet);
                        db.SaveChanges();
                        tmpStr = order.OID + "," + product.PID + "," + count.ToString() + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
                    }
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
                order.UID = null;
                order.Postage = 0;
                order.Subscription = 0;
                order.Status = (int)Order.EOrderStatus.OS_Init;
                order.SendType = (int)Order.ESendType.ST_Direct;
                order.PayType = (int)Order.EPayType.PT_Alipay;
                order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
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
                    }
                }
                //添加到数据库
                db.Orders.Add(order);
                db.OrderPackets.Add(packet);
                db.SaveChanges();
                tmpStr = order.OID.ToString() + "," + product.PID + "," + count.ToString() + "," + product.Title.Substring(0, Math.Min(20, product.Title.Length)) + "," + product.ImgPathArr[0] + ",";
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

        public ActionResult GetSelectProductInColum(int id)
        {
            List<Product> productLST = (from recp in db.ReColumnProducts
                                        where recp.ColumnID == id
                                        select recp.Product).ToList();
            if (Request.IsAjaxRequest())
            {
                string jsonStr = PubFunction.BuildResult(productLST);
                return Content(jsonStr);
            }
            else
            {
                return View();
            }
        }

        public ActionResult SetSelectProductInColum(int id, List<ReColumnProduct> Par)
        {
            db.m_objcontext.ExecuteStoreCommand("DELETE ReColumnProducts WHERE ColumnID = " + id);
            foreach (ReColumnProduct item in Par)
            {
                db.ReColumnProducts.Add(item);
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

        public ActionResult SetSelectProductInfo(Product Product,string ImgPath)
        {
            Product tmpProduct = db.Products.FirstOrDefault(r => r.PID == Product.PID);
            tmpProduct.Title = Product.Title;
            tmpProduct.MarketPrice = Product.MarketPrice;
            tmpProduct.Price = Product.Price;
            tmpProduct.Discount = Product.Discount;
            tmpProduct.Sale = Product.Sale;
            tmpProduct.Stock = Product.Stock;
            tmpProduct.PVCount = Product.PVCount;
            tmpProduct.Date = Product.Date;
            if (!string.IsNullOrEmpty(ImgPath))
            {
                string[] tmpArr = tmpProduct.ImgPathArr;
                tmpProduct.ImgPath = ImgPath + ";";
                for(int i=1;i<tmpArr.Length;i++)
                {
                    tmpProduct.ImgPath += tmpArr[i] + ";";
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
        public ActionResult EditDetail(int ID)
        {
            ProductEditDetailVM vmModel = new ProductEditDetailVM();
            //TODO:判断是否是自己的商品
            Product tmpProduct = db.Products.First(r => r.PID == ID);
            ViewBag.EvaluationCount = db.Evaluations.Count(r => r.Product.PID == ID);
            ViewBag.SaleCount = (from op in db.OrderPackets
                                    where op.PID == ID && op.Order.Status > 1
                                    select op.Count).ToList().Sum();
            tmpProduct.Category = GetSerialCategory(tmpProduct.CID);
            vmModel.Product = tmpProduct;

            //获得评论
            int[] NCStatus = new int[] { (int)Order.EOrderStatus.OS_Init };
            List<OrderPacket> tmpOPLST = db.OrderPackets.Where(r => r.PID == ID && !NCStatus.Contains(r.Order.Status)).ToList();
            List<int> tmpOIDArr = (from opl in tmpOPLST
                                    select opl.Order.OID).Distinct().ToList();
            //只取前20条
            List<Evaluation> tmpELST = db.Evaluations.Where(r => tmpOIDArr.Contains(r.OID)).OrderByDescending(r => r.DataTime).Take(20).ToList();
            vmModel.EvaluationLST = new List<EvaluationVM>();
            foreach (Evaluation eva in tmpELST)
            {
                EvaluationVM tmpEva = new EvaluationVM(eva);
                vmModel.EvaluationLST.Add(tmpEva);
            }
            List<ShamOrderData> tmpSODLST = db.ShamOrderDatas.Where(r => r.ProductID == ID).OrderByDescending(r => r.DateTime).Take(20).ToList();
            foreach (ShamOrderData sham in tmpSODLST)
            {
                EvaluationVM tmpEva = new EvaluationVM(sham);
                vmModel.EvaluationLST.Add(tmpEva);
            }
            vmModel.EvaluationLST.Sort((a, b) => b.DataTime.CompareTo(a.DataTime));

            //获得销售记录
            string DTFormat = "yyyy-MM-dd hh:mm:ss";
            string DTF = "M月d日";
            string DTStr = DateTime.Now.AddMonths(-1).ToString(DTFormat);
            tmpOPLST = db.OrderPackets.Where(r => r.PID == ID && !NCStatus.Contains(r.Order.Status) && r.Order.OrderDate.CompareTo(DTStr) > 0).ToList();
            DateTime tmpDT = DateTime.Now;
            string EDTStr = tmpDT.ToString(DTF);
            string BDTStr;
            string BDT, EDT = tmpDT.ToString(DTFormat);
            SaleLogVM tmpSLVM = new SaleLogVM();
            tmpSLVM.BDTStrArr = new List<string>();
            tmpSLVM.EDTStrArr = new List<string>();
            tmpSLVM.DTStrArr = new List<string>();
            tmpSLVM.CountArr = new List<int>();
            tmpSLVM.ShamCountArr = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                tmpDT = tmpDT.AddDays(-7);
                BDT = tmpDT.ToString(DTFormat);
                BDTStr = tmpDT.ToString(DTF);
                var opArr = from op in tmpOPLST
                            where op.Order.OrderDate.CompareTo(BDT) > 0 && op.Order.OrderDate.CompareTo(EDT) <= 0
                            select op;
                int shamCount = db.ShamOrderDatas.Count(r => r.Product.PID == ID && r.DateTime.CompareTo(BDT) > 0 && r.DateTime.CompareTo(EDT) <= 0);
                tmpSLVM.BDTStrArr.Insert(0, BDT);
                tmpSLVM.EDTStrArr.Insert(0, EDT);
                string tmpDTStr = string.Format("\"{0}\" 至 \"{1}\"", BDTStr, EDTStr);
                tmpSLVM.DTStrArr.Insert(0, tmpDTStr);
                tmpSLVM.CountArr.Insert(0, opArr.Sum(r => r.Count));
                tmpSLVM.ShamCountArr.Insert(0, shamCount);
                EDT = BDT;
                EDTStr = BDTStr;
            }
            vmModel.SaleLog = tmpSLVM;
            return View(vmModel);
        }

        public ActionResult AddItemDetail()
        {
            ProductEditDetailVM vmModel = new ProductEditDetailVM();
            //获得销售记录
            string DTFormat = "yyyy-MM-dd hh:mm:ss";
            string DTF = "M月d日";
            string DTStr = DateTime.Now.AddMonths(-1).ToString(DTFormat);
            DateTime tmpDT = DateTime.Now;
            string EDTStr = tmpDT.ToString(DTF);
            string BDTStr;
            string BDT, EDT = tmpDT.ToString(DTFormat);
            SaleLogVM tmpSLVM = new SaleLogVM();
            tmpSLVM.BDTStrArr = new List<string>();
            tmpSLVM.EDTStrArr = new List<string>();
            tmpSLVM.DTStrArr = new List<string>();
            tmpSLVM.CountArr = new List<int>();
            tmpSLVM.ShamCountArr = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                tmpDT = tmpDT.AddDays(-7);
                BDT = tmpDT.ToString(DTFormat);
                BDTStr = tmpDT.ToString(DTF);
                tmpSLVM.BDTStrArr.Insert(0, BDT);
                tmpSLVM.EDTStrArr.Insert(0, EDT);
                string tmpDTStr = string.Format("\"{0}\" 至 \"{1}\"", BDTStr, EDTStr);
                tmpSLVM.DTStrArr.Insert(0, tmpDTStr);
                tmpSLVM.CountArr.Insert(0, 0);
                tmpSLVM.ShamCountArr.Insert(0, 0);
                EDT = BDT;
                EDTStr = BDTStr;
            }
            vmModel.SaleLog = tmpSLVM;
            return View(vmModel);
        }

        public ActionResult SaveAddDetail(Product product, List<ShamOrderData> ShamOrderDataArr)
        {
            Product tmpProduct = new Product();
            tmpProduct.Title = product.Title;
            tmpProduct.EvaluationStarCount = product.EvaluationStarCount;
            tmpProduct.Price = product.Price;
            tmpProduct.MarketPrice = product.MarketPrice;
            tmpProduct.Sale = product.Sale;
            tmpProduct.Chose = product.Chose;
            tmpProduct.Descript = product.Descript;
            tmpProduct.ImgPath = product.ImgPath;
            //设置为其他
            Brand tmpBrand = db.Brands.FirstOrDefault(r => r.NameStr == "未知");
            if(tmpBrand == null)
            {
                tmpBrand = new Brand();
                tmpBrand.CountryCode = 0;
                tmpBrand.Img = "";
                tmpBrand.Important = 0;
                tmpBrand.Name2 = "未知";
                tmpBrand.NameStr = "未知";
                tmpBrand.Tag = 999;
                db.Brands.Add(tmpBrand);
            }
            tmpProduct.Brand = tmpBrand;
            //设置为首页
            Category tmpCategory = db.Categorys.FirstOrDefault(r => r.NameStr == "未知");
            if (tmpCategory == null)
            {
                tmpCategory = new Category();
                tmpCategory.NameStr = "未知";
                tmpCategory.Parent = tmpCategory;
                tmpCategory.Tag = 999;
                db.Categorys.Add(tmpCategory);
            }
            tmpProduct.Category = tmpCategory;
            db.Products.Add(tmpProduct);

            if (ShamOrderDataArr != null)
            {
                foreach (ShamOrderData item in ShamOrderDataArr)
                {
                    db.ShamOrderDatas.Add(item);
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

        public ActionResult SaveEditDetail(Product product, List<ShamOrderData> ShamOrderDataArr)
        {
            Product tmpProduct = db.Products.FirstOrDefault(r => r.PID == product.PID);
            tmpProduct.Title = product.Title;
            tmpProduct.EvaluationStarCount = product.EvaluationStarCount;
            tmpProduct.Price = product.Price;
            tmpProduct.MarketPrice = product.MarketPrice;
            tmpProduct.Sale = product.Sale;
            tmpProduct.Chose = product.Chose;
            tmpProduct.Descript = product.Descript;
            tmpProduct.ImgPath = product.ImgPath;

            if(ShamOrderDataArr != null)
            {
                foreach (ShamOrderData item in ShamOrderDataArr)
                {
                    db.ShamOrderDatas.Add(item);
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

        public ActionResult DelShamEvaluation(int EID)
        {
            db.ShamOrderDatas.Remove(db.ShamOrderDatas.FirstOrDefault(r => r.SOID == EID));
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

        public ActionResult EditEvaluation(EvaluationVM evaluation)
        {
            if(evaluation.IsSham)
            {
                ShamOrderData tmpSOD = db.ShamOrderDatas.FirstOrDefault(r => r.SOID == evaluation.EID);
                tmpSOD.Description = evaluation.Description;
            }
            else
            {
                Evaluation tmpEva = db.Evaluations.FirstOrDefault(r=>r.EID == evaluation.EID);
                tmpEva.Description = evaluation.Description;
                tmpEva.IsShow = evaluation.IsShow;
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

        public ActionResult DelProductsByCIDArr(int[] CIDArr)
        {
            string CIDArrStr = "";
            foreach (int CID in CIDArr)
            {
                CIDArrStr += CID + ",";
            }
            CIDArrStr = CIDArrStr.TrimEnd(new char[] { ',' });
            db.m_objcontext.ExecuteStoreCommand("DELETE Products WHERE CID IN (" + CIDArrStr + ")");
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

        public ActionResult DelProductsByPIDArr(int[] PIDArr)
        {
            string PIDArrStr = "";
            foreach(int PID in PIDArr)
            {
                PIDArrStr += PID + ",";
            }
            PIDArrStr = PIDArrStr.TrimEnd(new char[] { ',' });
            db.m_objcontext.ExecuteStoreCommand("DELETE Products WHERE PID IN (" + PIDArrStr + ")");
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
        public ActionResult ShowProductsByCIDArr(int[] CIDArr, int ShowTag)
        {
            string CIDArrStr = "";
            foreach (int CID in CIDArr)
            {
                CIDArrStr += CID + ",";
            }
            CIDArrStr = CIDArrStr.TrimEnd(new char[] { ',' });
            db.m_objcontext.ExecuteStoreCommand("UPDATE Products SET ShowTag = " + ShowTag + " WHERE CID IN (" + CIDArrStr + ")");
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

        public ActionResult ShowProductsByPIDArr(int[] PIDArr,int ShowTag)
        {
            string PIDArrStr = "";
            foreach (int PID in PIDArr)
            {
                PIDArrStr += PID + ",";
            }
            PIDArrStr = PIDArrStr.TrimEnd(new char[] { ',' });
            db.m_objcontext.ExecuteStoreCommand("UPDATE Products SET ShowTag = " + ShowTag + " WHERE PID IN (" + PIDArrStr + ")");
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
        public ActionResult BuildProductsXML(string PIDArrStr)
        {
            if (string.IsNullOrEmpty(PIDArrStr))
                Redirect("/Manager/ProductManager");
            if (string.IsNullOrEmpty(PIDArrStr))
                return View();
            string[] PIDArr = PIDArrStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] tmpPIDArr = new int[PIDArr.Length];
            for(int i=0;i<PIDArr.Length;i++)
            {
                tmpPIDArr[i] = int.Parse(PIDArr[i]);
            }
            Product[] productArr = (from product in db.Products
                                    where tmpPIDArr.Contains(product.PID)
                                   select product).ToArray();
            //生成所需的商品XML数据
            string FileName = "Product_" + Guid.NewGuid().ToString() + ".xlsx";
            string serverFP = PubFunction.GetUploadFilePathUsingDate();
            string localFP = Server.MapPath(serverFP);
            if (!Directory.Exists(localFP))
                Directory.CreateDirectory(localFP);
            Dictionary<string, MemberToStringDG> dict = new Dictionary<string, MemberToStringDG>();
            dict.Add("ProductTags", new MemberToStringDG(MTSHelper.ListToString));
            dict.Add("SaleCountLST", new MemberToStringDG(MTSHelper.ListToString));
            byte[] tmpBuffer = PubFunction.SaveToExcel<Product>(productArr, dict);

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/octet-stream";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(FileName));
            Response.BinaryWrite(tmpBuffer);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        } 

        
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadProductsExcel(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string tmpStr = "";
                    if (Request.Params.AllKeys.Contains("toPath"))
                    {
                        tmpStr = Request.Params["toPath"].ToString();
                    }
                    else
                    {
                        tmpStr = PubFunction.GetUploadFilePathUsingDate();
                    }
                    string filePath = Server.MapPath("/Uploads/");
                    if (!string.IsNullOrEmpty(tmpStr))
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
                    tmpStr = tmpStr.TrimStart(new char[] { '~' });
                    
                    //处理上传的Excel文件
                    Dictionary<string, StringToMemberDG> dict = new Dictionary<string, StringToMemberDG>();
                    dict.Add("ProductTags", new StringToMemberDG(MTSHelper.ListToString));
                    dict.Add("SaleCountLST", new StringToMemberDG(MTSHelper.ListToString));
                    IEnumerable<Product> productArr = PubFunction.LoadFromExcel<Product>(filePath + saveName,dict);
                    if(productArr == null)
                    {
                        //文件不存在
                    }
                    else
                    {
                        foreach(Product item in productArr)
                        {
                            db.Products.Add(item);
                        }
                        db.SaveChanges();
                    }

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
}