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
using FionPushFilm.Models;
using FionPushFilm.HtmlHelper;

namespace FionPushFilm.Controllers
{
    public class HomeController : Controller
    {
        private FilmDbContext db = new FilmDbContext();

        private static string HOMEPAGEURL = "http://www.torrentkitty.org";
        private static string SEARCHHTMLFORMAT = HOMEPAGEURL + "/search/{0}/{1}";
        //
        // GET: /Home/

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

        //[RequireHttps]
        public ActionResult Error(string id)
        {
            int tmpI = 403;
            int.TryParse(id, out tmpI);
            ViewBag.ErrCode = tmpI;
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

        public ActionResult SearchResource(string searchText,int pageIndex)
        {
            if (pageIndex <= 1)
            {
                SearchLog tmpLog = new SearchLog();
                tmpLog.IPAddress = Request.UserHostAddress;
                tmpLog.LogDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tmpLog.SearchStr = searchText;
                db.SearchLogs.Add(tmpLog);
                db.SaveChanges();
            }

            SearchResult result = new SearchResult();
            result.PageIndex = pageIndex;

            string htmlStr = HtmlReader.OpenSync(string.Format(SEARCHHTMLFORMAT, searchText, pageIndex));
            if(!string.IsNullOrEmpty(htmlStr))
            {

                HtmlAnalyser analyser = new HtmlAnalyser(htmlStr);
                HtmlAnalyser.MagnetResult[] tmpMC = analyser.GetResult();
                result.Items = new List<ResourceItem>();
                foreach (HtmlAnalyser.MagnetResult item in tmpMC)
                {
                    ResourceItem resourceItem = new ResourceItem();
                    resourceItem.ResourceName = item.Description;
                    resourceItem.MagnetLink = item.MargnetLink;
                    resourceItem.Date = item.Date;
                    resourceItem.Size = item.Size;
                    resourceItem.SeedLink = item.SeedLink;
                    resourceItem.DetailUrl = HOMEPAGEURL + item.DetailLink;
                    result.Items.Add(resourceItem);
                }
                result.PageCount = analyser.GetPageCount();
            }

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
