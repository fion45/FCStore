using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FCStore.Models;

using System.Drawing;
using System.IO;

namespace FCStore.Controllers
{
    public class HomeController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(db.Columns.ToList());
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

            int iWidth = str.Length * 11;
            Bitmap img = new Bitmap(iWidth, 20);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            Color[] colors = new Color[] { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };
            for (int i = 0; i < str.Length; i++)
            {
                Color c = colors[rand.Next(0, colors.Length)];
                Font f = new Font("Courier New", 11);
                Brush b = new System.Drawing.SolidBrush(c);

                //画字符
                g.DrawString(str.Substring(i, 1), f, b, (i * 10) + 1, 1, StringFormat.GenericDefault);
            }

            //描边
            g.DrawRectangle(new Pen(Color.Black), 0, 0, img.Width - 1, img.Height - 1);


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
