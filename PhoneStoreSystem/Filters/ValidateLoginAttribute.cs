using System.Web.Mvc;
using System.Web.Routing;

namespace PhoneStoreSystem.Filters
{
    //这个是用于简单的判断用户是否登录的
    public class ValidateLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserName"] == null)
            {
                string redirectOnSuccess = filterContext.HttpContext.Request.RawUrl;
                //string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = "/Account/Login";               
                //filterContext.HttpContext.Response.Redirect(loginUrl, true);
                filterContext.Result = new RedirectResult(loginUrl, true);
                //filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
        }
    }
}