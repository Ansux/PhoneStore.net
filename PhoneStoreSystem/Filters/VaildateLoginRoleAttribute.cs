using PhoneStoreSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace PhoneStoreSystem.Filters
{
    //继承拦截器类，用于判断用户是否属于要求的角色
    public class VaildateLoginRoleAttribute : ActionFilterAttribute
    {
        //
        /// 角色名称
        ///
        public string Role { get; set; }

       

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /*
            int nameId = WebSecurity.CurrentUserId;
            UsersContext uc = new UsersContext();
            if (uc.UserProfiles.Find(nameId).UserRole.ToLower().Equals("visitor"))
            {
                throw new HttpException(403, "权限出问题了！");
            }
            */
            if (filterContext.HttpContext.Session["UserName"] != null) {
                if (!string.IsNullOrEmpty(Role))
                {
                    //判断是否存在角色
                    string role= filterContext.HttpContext.Session["UserRoleName"] as string;

                    string[] chkRoles = this.Role.Split(',');
                    bool isAuthorized = false;
                    if (Array.IndexOf(chkRoles, role) > -1)
                        isAuthorized = true;
                    else
                        isAuthorized = false;

                    if (!isAuthorized)
                    {
                        filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Admin", action = "NoAccess", id = Role }));
                    }
                    //throw new UnauthorizedAccessException("你没有权限访问该页面");
                }
            }else{

                 filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
            
        }
    }

    //错误验证
    //public class ErrorAttribute : ActionFilterAttribute
    //{
    // public override void OnActionExecuted(ActionExecutedContext filterContext) // OnActionExecuted表示在Action执行之后
    // {
    // if (filterContext.Exception != null)
    // {
    // filterContext.ExceptionHandled = true;
    // filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Shared", action = "Error" }));
    // }
    // }
    //}

    
}