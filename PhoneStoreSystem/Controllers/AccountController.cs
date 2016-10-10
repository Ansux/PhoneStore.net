using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using System.Web.Security;

namespace PhoneStoreSystem.Controllers
{

    public class AccountController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /UsersDb/
       
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                
                var u = from n in db.UserInfos 
                        where n.UserName == model.UserName && n.PassWord == model.Password 
                        select n;
                if (u.Count()!=0) {
                    UserInfo userInfo = u.First();
                 
                    //FormsAuthentication.SetAuthCookie(userInfo.UserName,true);

                    Session.Add("UserId", userInfo.UserId);
                    Session.Add("UserName", userInfo.UserName);
                    Session.Add("UserRoleName", userInfo.RoleInfo.RoleName);

                    return RedirectToLocal(returnUrl);
                }
     
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View(model);
         
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["UserId"] = null;
            Session["UserName"] = null;            
            Session["UserRoleName"] = null;

            //取消Session会话
            //Session.Abandon();
 
            //删除Forms验证票证
            //FormsAuthentication.SignOut();
            
            return RedirectToAction("Login", "Account");
        }

        //用于检测用户名是否存在
        public JsonResult IsExistUser(string userName) {
            
            bool isExist = true;

            var u = from n in db.UserInfos
                    where n.UserName == userName
                    select n; ;
            if (u.Count() != 0)
            {
                isExist=false;
            }

            return Json(isExist, JsonRequestBehavior.AllowGet);    
        }
        
        
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                try
                {
                    UserInfo u = new UserInfo();
                    u.UserName = model.UserName;
                    u.PassWord = model.Password;
                    u.RoleId = db.RoleInfos.First().RoleId;
                    db.UserInfos.Add(u);
                    db.SaveChanges();
                    //Session.Add("User", u); 
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "注册失败!");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        private ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
         
    }
}