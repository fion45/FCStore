using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FionPushFilm.Filters;
using FionPushFilm.Common;
using System.Text;
using System.Drawing;
using System.IO;

namespace FionPushFilm.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        [MyAuthorizeAttribute]
        public ActionResult Index()
        {
            return View();
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
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.myUser = User as MyUser;
            }
            return View();
        }

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
    }
}
