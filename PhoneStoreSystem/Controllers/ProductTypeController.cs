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
    public class ProductTypeController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /ProductType/

        public ActionResult Index()
        {
            return View(db.ProductTypes.ToList());
        }

        //
        // GET: /ProductType/Details/5

        public ActionResult Details(int id = 0)
        {
            ProductType producttype = db.ProductTypes.Find(id);
            if (producttype == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(producttype);
        }

        //
        // GET: /ProductType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProductType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductType producttype)
        {
            if (ModelState.IsValid)
            {
                db.ProductTypes.Add(producttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(producttype);
        }

        //
        // GET: /ProductType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProductType producttype = db.ProductTypes.Find(id);
            if (producttype == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(producttype);
        }

        //
        // POST: /ProductType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductType producttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producttype);
        }

        //用于检测是否存在
        public JsonResult IsExistProType(string typeName)
        {           
            bool isExist = true;
            try
            {
                string[] url = Request.UrlReferrer.LocalPath.Split('/');
                string actionName = url[2];
                if (actionName == "Create")
                {
                    var u = from n in db.ProductTypes
                            where n.TypeName == typeName
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
                else if (actionName == "Edit")
                {
                    int id = int.Parse(url[url.Length - 1]);

                    var u = from n in db.ProductTypes
                            where n.TypeName == typeName && n.TypeId!=id 
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
            }
            catch {              
                var u = from n in db.ProductTypes
                        where n.TypeName == typeName
                        select n; ;
                if (u.Count() != 0)
                {
                    isExist = false;
                }               
            }
                      
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
        

        //
        // GET: /ProductType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ProductType producttype = db.ProductTypes.Find(id);
            if (producttype == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(producttype);
        }

        //
        // POST: /ProductType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductType producttype = db.ProductTypes.Find(id);
            db.ProductTypes.Remove(producttype);
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