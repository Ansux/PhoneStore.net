using NewsSystem.Controllers;
using PhoneStoreSystem.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneStoreSystem.Controllers
{

    [VaildateLoginRole(Role = ("Administrator,Manager"))]
    public class LogoSetController : Controller
    {
        private static string imgName = "";

        //
        // GET: /LogoSet/

        public ActionResult Index()
        {
            ViewBag.ImagUrl = "logo_img.png";
            return View();
        }

        //异步上传图片
        public ActionResult FileImgUpload()
        {
            HttpPostedFileBase imgFile = Request.Files[0];
            long filename = PublicController.TimeStamp();
            Image bigImg = null;
            try
            {
                //保存上传的图片
                imgFile.SaveAs(Server.MapPath("../Images/web/upload/" + filename + ".png"));
                //然后再设置图片大小
                bigImg = Image.FromFile(Server.MapPath("../Images/web/upload/" + filename + ".png"));
                Bitmap b = PublicController.SizeImage(bigImg, 200, 120);
                b.Save(Server.MapPath("../Images/web/logo_img.png"));
                b.Dispose();
            }
            catch (FileNotFoundException e)
            {
                Response.Write(e.Message);
                ViewBag.StatusMessage = "上传失败，请联系开发员！";
            }
            finally
            {
                if (bigImg != null)
                    bigImg.Dispose();
            }
            imgName = "logo_img.png";
            return Content("/Images/web/upload/" + filename + ".png");
        }


    }
}
