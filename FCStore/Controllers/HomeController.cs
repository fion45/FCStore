using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using FCStore.Filters;
using FCStore.Common;

namespace FCStore.Controllers
{
    public class HomeController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //清除不存在的order
            if (Request.Cookies.AllKeys.Contains("Order"))
            {
                Regex cookieRgx = new Regex(FCStore.Controllers.ProductController.ORDERCOOKIERGX);
                Match tmpMatch = cookieRgx.Match(Server.UrlDecode(Request.Cookies["Order"].Value));
                if (!string.IsNullOrEmpty(tmpMatch.Value))
                {
                    int OrderID = int.Parse(tmpMatch.Groups["ORDERID"].Value);
                    Order order = db.Orders.FirstOrDefault(r => r.OID == OrderID);
                    if(order == null)
                    {
                        //设置cookie过时
                        Response.Cookies["Order"].Expires = DateTime.MinValue;
                    }
                }
            }
            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.User.IsInRole("admin"))
            {
                ViewBag.IsAdmin = true;
            }
            ViewBag.BannerItems = db.BannerItems.OrderBy(r=>r.Index).ToList();
            return View(db.Columns.ToList());
        }

        [LoginActionFilterAttribute(beforeTag = -1)]
        //[RequireHttps]
        public ActionResult Login(string id)
        {
            ViewBag.returnUrl = "";
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.returnUrl = Encoding.UTF8.GetString(Convert.FromBase64String(id));
            }
            if(User.Identity.IsAuthenticated)
            {
                ViewBag.myUser = User as MyUser;
            }
            return View();
        }

        //[RequireHttps]
        public ActionResult Error(string id)
        {
            ViewBag.ErrCode = id;
            return View();
        }

        //[RequireHttps]
        public ActionResult Register(string id)
        {
            ViewBag.returnUrl = "";
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.returnUrl = Encoding.UTF8.GetString(Convert.FromBase64String(id));
            }
            return View();
        }

        public ActionResult GetValidateCode()
        {
            byte[] bytes = CreateValidateGraphic();
            return File(bytes, @"image/jpeg");
        } 

        public byte[] CreateValidateGraphic()
        {
            string str = "0123456789";
            char[] chs = str.ToCharArray();
            Random rand = new Random((int)DateTime.Now.Ticks);

            string validater = "";
            for (int i = 0; i < 4; i++)
            {
                char x = chs[rand.Next(0, chs.Length)];
                validater += x;
            }
            Session["Validate_code"] = validater;

            int iWidth = validater.Length * 32;
            Bitmap img = new Bitmap(iWidth, 40);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            Color[] colors = new Color[] { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };
            for (int i = 0; i < validater.Length; i++)
            {
                Color c = colors[rand.Next(0, colors.Length)];
                Font f = new Font("Courier New", 25, FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c);

                //画字符
                g.DrawString(validater.Substring(i, 1), f, b, (i * 32) + 1, 1, StringFormat.GenericDefault);
            }

            //描边
            //g.DrawRectangle(new Pen(Color.Black), 0, 0, img.Width - 1, img.Height - 1);


            //保存图片数据
            MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            g.Dispose();
            img.Dispose();
            //输出图片流
            return stream.ToArray();

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
