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
    [VaildateLoginRole(Role = ("Administrator"))]
    public class UserManageController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /UserManage/

        public ActionResult Index()
        {
            
            var userInfos = from n in db.UserInfos where n.RoleInfo.RoleName!= "Administrator" select new { n.UserName };



            ViewBag.listCount = userInfos.ToList().Count();
            return View();
        }

        public ActionResult GetPageList(int pageIndex = 0, int pageSize = 10)
        {

            db.Configuration.ProxyCreationEnabled = false;

            var userName = Session["UserName"];
            var userInfos = db.UserInfos.Where(n=>n.RoleInfo.RoleName != "Administrator").Include(u => u.RoleInfo);
         
            //var productinfos = db.ProductInfos.Include(p => p.Pstyle);
            return Json(userInfos.ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /UserManage/Details/5

        public ActionResult Details(int id = 0)
        {
            UserInfo userinfo = db.UserInfos.Find(id);
            if (userinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(userinfo);
        }

        //
        // GET: /UserManage/Create

        public ActionResult Create()
        {
            //ViewBag.UserRoles = getRoleList("Visitor");
            ViewBag.RoleId = new SelectList(db.RoleInfos.Where(r => r.RoleName != "Administrator"), "RoleId", "RoleName");
            return View();
        }

        //
        // POST: /UserManage/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserInfo userinfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.UserInfos.Add(userinfo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }catch{
                    ModelState.AddModelError("", "注册失败!");
                }
            }

            ViewBag.RoleId = new SelectList(db.RoleInfos.Where(r => r.RoleName != "Administrator"), "RoleId", "RoleName", userinfo.RoleId);
            return View(userinfo);
        }
       

        //
        // GET: /UserManage/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserInfo userinfo = db.UserInfos.Find(id);
            if (userinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.RoleId = new SelectList(db.RoleInfos.Where(r => r.RoleName != "Administrator"), "RoleId", "RoleName", userinfo.RoleId);
            return View(userinfo);
        }

        //
        // POST: /UserManage/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserInfo userinfo)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(userinfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "修改失败!");
                }
                   
            }

            ViewBag.RoleId = new SelectList(db.RoleInfos.Where(r => r.RoleName != "Administrator"), "RoleId", "RoleName", userinfo.RoleId);
            return View(userinfo);
        }

        //
        // GET: /UserManage/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserInfo userinfo = db.UserInfos.Find(id);
            if (userinfo == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(userinfo);
        }

        //
        // POST: /UserManage/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserInfo userinfo = db.UserInfos.Find(id);
            if (userinfo.UserName == (string)Session["UserName"]) {               
                ModelState.AddModelError("", "不能对当前登录的用户进行删除操作!");
                return View(userinfo);
            }
            try
            {             
                db.UserInfos.Remove(userinfo);
                db.SaveChanges();                
            }
            catch
            {
                ModelState.AddModelError("", "删除失败，请联系管理员!");              
                return View(userinfo);               
            }
    
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}