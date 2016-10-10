using System.Web.Mvc;
using System.Web.Routing;

namespace PhoneStoreSystem.Filters
{
    //这个是用于简单的判断用户是否登录的
    public class ValidateMemberLoginAttribute : ActionFilterAttribute
    {
  

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
               || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }
            if (filterContext.HttpContext.Session["MemberId"] == null)
            {
                string redirectOnSuccess = filterContext.HttpContext.Request.RawUrl;
                string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = "/Web/MemberLogin/" + redirectUrl;
                //filterContext.HttpContext.Response.Redirect(loginUrl,true);
                filterContext.Result = new RedirectResult(loginUrl, true);
                //; new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Web", action = "MemberLogin" }));
            }
        }
    }
}