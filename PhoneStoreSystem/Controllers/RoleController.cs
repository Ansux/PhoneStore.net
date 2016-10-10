using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using PhoneStoreSystem.Filters;

namespace PhoneStoreSystem.Controllers
{
    [ValidateLogin]
    public class RoleController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View(db.RoleInfos.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(int id = 0)
        {
            RoleInfo roleinfo = db.RoleInfos.Find(id);
            if (roleinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(roleinfo);
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleInfo roleinfo)
        {
            if (ModelState.IsValid)
            {
                db.RoleInfos.Add(roleinfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roleinfo);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(int id = 0)
        {
            RoleInfo roleinfo = db.RoleInfos.Find(id);
            if (roleinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(roleinfo);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoleInfo roleinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roleinfo);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id = 0)
        {
            RoleInfo roleinfo = db.RoleInfos.Find(id);
            if (roleinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(roleinfo);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleInfo roleinfo = db.RoleInfos.Find(id);
            db.RoleInfos.Remove(roleinfo);
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