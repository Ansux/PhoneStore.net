
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using System.Data;
using PhoneStoreSystem.Filters;

namespace PhoneStoreSystem.Controllers
{
    //[VaildateLoginRole(Role = ("Administrator,Manager"))]
    [ValidateLogin]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        PhoneStoreContext uc = new PhoneStoreContext();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult About()
        {                    
            return View();
        }

        public ActionResult DataCount() {

            ViewBag.ProductTypeCount = uc.ProductTypes.Count();
            ViewBag.ProductSaleCount = uc.ProductInfos.Where(p=>p.ProIsSale==true).Count();
            ViewBag.ProductNoSaleCount = uc.ProductInfos.Where(p => p.ProIsSale == false).Count();
            
            var salas=uc.ProductInfos.Select(p => p.ProSales);
            if (salas.Count()!=0)
                ViewBag.ProductSalesCount = salas.Sum();
            else
                ViewBag.ProductSalesCount = 0;

            var storage = uc.ProductStorages.Select(p => p.StorageNumber);
            if (storage.Count() != 0)
                ViewBag.ProductStorageCount = storage.Sum();
            else
                ViewBag.ProductStorageCount = 0;

            ViewBag.OrdersCount = uc.Orders.Count();
            ViewBag.MembersCount = uc.Members.Count();
            ViewBag.UserAdmin = uc.UserInfos.Where(i => i.RoleInfo.RoleName
                .Equals("Administrator")).Select(i=>i.UserName);
            return View();
        }
        
        public ActionResult MyInfo()
        {
            int id= int.Parse(Session["UserId"].ToString());
            var ui = uc.UserInfos.Where(i => i.UserId ==id).First();

            return View(ui);
        }
        
        public ActionResult EditMyInfo(int id)
        {
            var ui = uc.UserInfos.Where(i => i.UserId ==id).First();
            UserProfile up = new UserProfile()
            {
                UserId=ui.UserId,
                RealName = ui.RealName,
                UserEmail = ui.UserEmail
            };
            return View(up);
        }       
        [HttpPost]
        public ActionResult EditMyInfo(UserProfile userProfile)
        {
            try{
                if (ModelState.IsValid){
                    UserInfo userInfo = uc.UserInfos.Where(i => i.UserId == userProfile.UserId).First();
                    userInfo.RealName = userProfile.RealName;
                    userInfo.UserEmail = userProfile.UserEmail;
                    uc.Entry(userInfo).State = EntityState.Modified;
                    uc.SaveChanges();
                    return RedirectToAction("MyInfo");
                }else{
                    ViewBag.newsInfoError = "数据出错，请检查！";
                }
            }catch{ 
                ViewBag.newsInfoError = "数据出错，请检查！"; 
            }
            return View(userProfile);
        }

        public ActionResult AlterPassword(string message)
        {
            ViewBag.StatusMessage = message;
            ViewBag.ReturnUrl = Url.Action("AlterPassword");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterPassword(LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("AlterPassword");

            if (ModelState.IsValid)
            {
                // 在某些失败方案中，ChangePassword 将引发异常，而不是返回 false。
                bool changePasswordSucceeded;
                try
                {
                    int id = int.Parse(Session["UserId"].ToString());
                    var userInfo = uc.UserInfos.Where(i => i.UserId == id).First();
                    if (userInfo.PassWord == model.OldPassword)
                    {
                        userInfo.PassWord = model.NewPassword;
                        uc.Entry(userInfo).State = EntityState.Modified;
                        uc.SaveChanges();
                        changePasswordSucceeded = true;
                    }
                    else {
                        changePasswordSucceeded = false;
                    }           
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)               
                    return RedirectToAction("AlterPassword", new { Message = "更改你的密码成功！" });                
                else                
                    ModelState.AddModelError("", "当前密码不正确或新密码无效。");                
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult NoAccess(string id)
        {
            ViewBag.AllowRoles = id;
            return View();
        }
        /*
        public ActionResult DataCount()
        {
            NewsModels db = new NewsModels();
            ViewBag.NewsTypeCount = db.NewsTypes.Count();
            ViewBag.NewsCount = db.NewsImageTexts.Count();
            ViewBag.UserCount = uc.UserProfiles.Count();
            ViewBag.CommentCount = db.NewsComments.Count();
            ViewBag.UserManager = uc.UserProfiles.Where(i => i.UserRole.Equals("Managers")).Count();
            ViewBag.UserAdmin = uc.UserProfiles.Where(i => i.UserRole.Equals("Administrator")).Count();
            return View();
        }
        */
    }
}
