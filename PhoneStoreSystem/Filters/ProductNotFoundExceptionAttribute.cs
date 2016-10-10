using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PhoneStoreSystem.Filters
{
    public class ProductNotFoundExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
          
            filterContext.ExceptionHandled = true;
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                //跳转到没有该产品的界面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Web", action = "index" }));
            }
            else {
                filterContext.HttpContext.Response.Write("系统出错了，请重新访问！");
            }
            
        }
    }
}