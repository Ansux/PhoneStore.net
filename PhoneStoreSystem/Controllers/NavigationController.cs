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
    [VaildateLoginRole(Role = ("Administrator,Manager"))]
    public class NavigationController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /Navigation/

        public ActionResult Index()
        {
            return View(db.Navigations.ToList());
        }

        //
        // GET: /Navigation/Details/5

        public ActionResult Details(int id = 0)
        {
            Navigation navigation = db.Navigations.Find(id);
            if (navigation == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(navigation);
        }

        //
        // GET: /Navigation/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Navigation/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Navigation navigation)
        {
            if (db.Navigations.Count() == 5)
            {
                ViewBag.StatusMessage = "对不起，已存在5个导航栏目！";
                return View(navigation);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Navigations.Add(navigation);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StatusMessage = "您的数据有误或名称字段出现重复，请检查！";
                }
            }
            catch
            {
                ViewBag.StatusMessage = "您的数据有误或名称字段出现重复，请检查！";
            }

            return View(navigation);
        }

        //
        // GET: /Navigation/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Navigation navigation = db.Navigations.Find(id);
            if (navigation == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(navigation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Navigation navigation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(navigation).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StatusMessage = "您的数据有误或名称字段出现重复，请检查！";
                }
            }
            catch
            {
                ViewBag.StatusMessage = "您的数据有误或名称字段出现重复，请检查！";
            }

            return View(navigation);
        }


        //用于检测是否存在
        public JsonResult IsExistNavigation(string navName)
        {
            bool isExist = true;
            try
            {
                string[] url = Request.UrlReferrer.LocalPath.Split('/');
                string actionName = url[2];
                if (actionName == "Create")
                {
                    var u = from n in db.Navigations where n.NavName == navName select n; 
                    if (u.Count() != 0)                    
                        isExist = false;
                }
                else if (actionName == "Edit")
                {
                    int id = int.Parse(url[url.Length - 1]);
                    var u = from n in db.Navigations where n.NavName == navName && n.NavId != id select n; 
                    if (u.Count() != 0)                    
                        isExist = false;                    
                }
            }
            catch
            {
                var u = from n in db.Navigations where n.NavName == navName select n; ;
                if (u.Count() != 0)                
                    isExist = false;                
            }
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Navigation/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Navigation navigation = db.Navigations.Find(id);
            if (navigation == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(navigation);
        }

        //
        // POST: /Navigation/Delete/5

        [HttpPost, ActionName("Delete")]    
        public ActionResult DeleteConfirmed(int id)
        {
            Navigation navigation = db.Navigations.Find(id);
            db.Navigations.Remove(navigation);
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