using PhoneStoreSystem.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;


namespace PhoneStoreSystem.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Error404() {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }
        public ActionResult Index()
        {


            ViewBag.Message = Session["UserName"];
            

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
