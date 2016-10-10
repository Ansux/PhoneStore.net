using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using PhoneStoreSystem.Filters;
using NewsSystem.Controllers;
using System.Drawing;
using System.IO;

namespace PhoneStoreSystem.Controllers
{
    [VaildateLoginRole(Role = ("Administrator,Manager"))]
    public class SlidesController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();
        private static string imgName = "";
        //
        // GET: /Slides/

        public ActionResult Index()
        {
            return View(db.Slides.ToList());
        }

        //
        // GET: /Slides/Details/5

        public ActionResult Details(int id = 0)
        {
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.ImagUrl = slide.SidImgUrl;
            return View(slide);
        }

        //
        // GET: /Slides/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Slides/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Slide slide)
        {
            if (db.Slides.Count() == 6)
            {
                ViewBag.StatusMessage = "对不起，已存在6张幻灯片图片！";
                return View(slide);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if (imgName != "")
                        slide.SidImgUrl = imgName;
                    db.Slides.Add(slide);
                    db.SaveChanges();
                    imgName = "";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StatusMessage = "您的数据有误，请检查！";
                }
            }
            catch
            {
                ViewBag.StatusMessage = "您的数据有误，请检查！";
                return View(slide);
            }

            return View(slide);
        }

        //
        // GET: /Slides/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.ImagUrl = slide.SidImgUrl;
            return View(slide);
        }

        //
        // POST: /Slides/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Slide slide)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imgName != "")
                        slide.SidImgUrl = imgName;
                    db.Entry(slide).State = EntityState.Modified;
                    db.SaveChanges();
                    imgName = "";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StatusMessage = "您的数据有误，请检查！";
                }
            }
            catch
            {
                ViewBag.StatusMessage = "您的数据有误，请检查！";
                ViewBag.ImagUrl = slide.SidImgUrl;
                return View(slide);
            }

            ViewBag.ImagUrl = slide.SidImgUrl;
            return View(slide);
        }

        //
        // GET: /Slides/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.ImagUrl = slide.SidImgUrl;
            return View(slide);
        }

        //
        // POST: /Slides/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slide slide = db.Slides.Find(id);
            db.Slides.Remove(slide);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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
                imgFile.SaveAs(Server.MapPath("../Images/web/upload/" + filename + ".jpeg"));

                //然后再设置图片大小
                bigImg = Image.FromFile(Server.MapPath("../Images/web/upload/" + filename + ".jpeg"));
               
                Bitmap b = PublicController.SizeImage(bigImg, 1000, 400);

                PublicController.SaveFormatJpeg(b, Server.MapPath("../Images/web/slideImg/" + filename + ".jpeg"), 100L);
               
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
            imgName = filename + ".jpeg";
            return Content("/Images/web/slideImg/" + imgName);
        }
    }
}