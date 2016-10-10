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
    public class ProductColorController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /ProductColor/

        public ActionResult Index()
        {
            return View(db.ProductColors.ToList());
        }

        //
        // GET: /ProductColor/Details/5

        public ActionResult Details(int id = 0)
        {
            ProductColor productcolor = db.ProductColors.Find(id);
            if (productcolor == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(productcolor);
        }

        //
        // GET: /ProductColor/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProductColor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductColor productcolor)
        {
            if (ModelState.IsValid)
            {
                db.ProductColors.Add(productcolor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productcolor);
        }

        //
        // GET: /ProductColor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProductColor productcolor = db.ProductColors.Find(id);
            if (productcolor == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(productcolor);
        }

        //
        // POST: /ProductColor/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductColor productcolor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productcolor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productcolor);
        }

        //用于检测是否存在
        public JsonResult IsExistProColor(string colorName)
        {
            bool isExist = true;
            try
            {
                string[] url = Request.UrlReferrer.LocalPath.Split('/');
                string actionName = url[2];
                if (actionName == "Create")
                {
                    var u = from n in db.ProductColors
                            where n.ColorName == colorName
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
                else if (actionName == "Edit")
                {
                    int id = int.Parse(url[url.Length - 1]);

                    var u = from n in db.ProductColors
                            where n.ColorName == colorName && n.ColorId != id
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
            }
            catch
            {
                var u = from n in db.ProductColors
                        where n.ColorName == colorName
                        select n; ;
                if (u.Count() != 0)
                {
                    isExist = false;
                }
            }

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
        
       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}