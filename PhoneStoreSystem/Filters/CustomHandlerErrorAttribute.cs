using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneStoreSystem.Filters
{
    public class CustomHandlerErrorAttribute : HandleErrorAttribute  
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.Controller.ViewData.Model = filterContext.Exception;

            if (filterContext.Exception == null)
            {
                filterContext.HttpContext.Response.StatusCode = 404;

                filterContext.Result = new ViewResult
                {
                    ViewName = "Error404",
                    ViewData = filterContext.Controller.ViewData
                };

            }
            else {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error500",
                    ViewData = filterContext.Controller.ViewData
                };

            }
            

            filterContext.ExceptionHandled = true;
        }  
    }
}