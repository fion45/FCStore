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
    public class UserController : Controller
    {
        private FCStoreDbContext db = new FCStoreDbContext();

        public ActionResult Login(string ValidateCode, string loginID, string loginPSW)
        {
            User user = null;
            if (ValidateCode != Session["Validate_code"])
            {
                ViewBag.LoginFail = -2;
            }
            else
            {
                try
                {
                    user = db.Users.Where(r => (r.LoginID == loginID && r.LoginPSW == loginPSW)).First();
                    ViewBag.LoginFail = 0;
                }
                catch
                {
                    ViewBag.LoginFail = -1;
                    user = null;
                }
            }
            if (Request.IsAjaxRequest())
            {
                string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(user);
                return Content(jsonStr);
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult Registe(User user)
        {


            return View(user);
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.RID = new SelectList(db.Roles, "RID", "RoleName");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RID = new SelectList(db.Roles, "RID", "RoleName", user.RID);
            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RID = new SelectList(db.Roles, "RID", "RoleName", user.RID);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RID = new SelectList(db.Roles, "RID", "RoleName", user.RID);
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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