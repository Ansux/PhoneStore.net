using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using NewsSystem.Controllers;
using System.Drawing;
using System.IO;
using PhoneStoreSystem.Filters;

namespace PhoneStoreSystem.Controllers
{
    [VaildateLoginRole(Role = ("Administrator"))]
    public class ProductMemberController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        private static string imgName = "";
        //
        // GET: /Member/

        public ActionResult Index(string MemberName = null, string RegisterDateStart = null, string RegisterDateEnd = null)
        {
            var members = from n in db.Members select n;

            ViewBag.memberName = MemberName;       
            ViewBag.startDate = RegisterDateStart;
            ViewBag.endDate = RegisterDateEnd;

            if (!String.IsNullOrEmpty(RegisterDateStart) && !String.IsNullOrEmpty(RegisterDateEnd))
            {
                DateTime startDate = Convert.ToDateTime(RegisterDateStart);
                DateTime endDate = Convert.ToDateTime(RegisterDateEnd);
                members = members.Where(i => i.MemberRegisterTime >= startDate).Where(i => i.MemberRegisterTime <= endDate);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(MemberName))
            {
                members = members.Where(i => i.MemberName.Contains(MemberName));
                ViewBag.pageIndex = 0;
            }

            ViewBag.listCount = members.ToList().Count();
            return View();
        }

        public ActionResult GetPageList(int pageIndex = 0, int pageSize = 10, string MemberName = null, string RegisterDateStart = null, string RegisterDateEnd = null)
        {
            //默认采用创建代理的方式返回实体集合，这里必须要关闭才可以读出数据
            db.Configuration.ProxyCreationEnabled = false;

            var members = from n in db.Members select n;

            if (!String.IsNullOrEmpty(RegisterDateStart) && !String.IsNullOrEmpty(RegisterDateEnd))
            {
                DateTime startDate = Convert.ToDateTime(RegisterDateStart);
                DateTime endDate = Convert.ToDateTime(RegisterDateEnd);
                members = members.Where(i => i.MemberRegisterTime >= startDate).Where(i => i.MemberRegisterTime <= endDate);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(MemberName))
            {
                members = members.Where(i => i.MemberName.Contains(MemberName));
                ViewBag.pageIndex = 0;
            }


            //var productinfos = db.ProductInfos.Include(p => p.Pstyle);
            return Json(members.OrderByDescending(i => i.MemberRegisterTime).ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /Member/Details/5

        public ActionResult Details(int id = 0)
        {
            Member member = db.Members.Find(id);
            if (member == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.ImagUrl = member.MemberPhoto;
            return View(member);
        }

        //
        // GET: /Member/Create

        public ActionResult Create()
        {
            imgName = "";
            ViewBag.MemberImage = "../Images/web/white.gif";

            return View();

        }

        //
        // POST: /Member/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            ViewBag.MemberImage = "../Images/web/white.gif";
            if (imgName == "")
            {

                ViewBag.ErrorMessage = "创建失败，请先点击上传图片！";

                return View(member);
            }
            else
            {
                ViewBag.MemberImage = imgName;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    member.MemberPhoto = imgName;
                    member.MemberRegisterTime = DateTime.Now;
                    db.Members.Add(member);
                    db.SaveChanges();
                    imgName = "";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e) { 
                
            }

            return View(member);
        }

        //
        // GET: /Member/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Member member = db.Members.Find(id);
            if (member == null)
            {
                throw new HttpException(404, "page not found");
            }
            imgName = "";
            ViewBag.MemberImage = member.MemberPhoto;
            return View(member);
        }

        //
        // POST: /Member/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member member)
        {
            var m = db.Members.Find(member.MemberId);
            try
            {
                
                if (imgName != "")
                    //将其他未输入的值，在这里赋值。
                    m.MemberPhoto = imgName;
                m.MemberPassword = member.MemberPassword;
                m.MemberRealName = member.MemberRealName;
                m.MemberPhone = member.MemberPhone;

                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                imgName = "";
                return RedirectToAction("Index");
                
            }
            catch (Exception e)
            {

            }

            if (imgName != "")
            {
                ViewBag.MemberImage = imgName;
            }
            return View(m);
        }

        //
        // GET: /Member/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Member member = db.Members.Find(id);
            if (member == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(member);
        }

        //
        // POST: /Member/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
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
            long filename = PublicController.TimeStamp();

            Image myImg = null;
            try
            {

                HttpPostedFileBase imgFile = Request.Files[0];

                string type = imgFile.ContentType.ToLower();

                if ((type == "image/png" || type == "image/jpeg") || type == "image/gif")
                {
                    //保存上传的图片
                    imgFile.SaveAs(Server.MapPath("../Images/web/upload/" + filename + ".png"));


                    //然后再设置图片大小
                    myImg = Image.FromFile(Server.MapPath("../Images/web/upload/" + filename + ".png"));
                    /*
                    Bitmap b = null;
                    int bl = myImg.Width / myImg.Height;
                    if (bl >= 1)
                    {
                        if (myImg.Width < 1000)
                        {
                            b = PublicController.PercentImage(myImg, 1);
                        }
                        else
                        {
                            b = PublicController.PercentImage(myImg, (double)(1000F / myImg.Width));
                        }
                    }
                    else
                    {
                        b = PublicController.PercentImage(myImg, (double)(750F / myImg.Height));
                    }


                    b.Save(Server.MapPath("../Images/web/big/" + filename + ".jpeg"));
                    PublicController.SaveFormatJpeg(b, Server.MapPath("../Images/web/big/" + filename + ".jpeg"), 100L);
                    b.Dispose();
                    */
                    Bitmap s = PublicController.SizeImage(myImg, 300, 300);

                    s.Save(Server.MapPath("../Images/web/memberImg/" + filename + ".png"));

                    //PublicController.SaveFormatJpeg(s, Server.MapPath("../Images/web/memberImg/" + filename + ".jpeg"), 100L);
                    s.Dispose();
                }
                else
                {
                    throw new HttpException();
                }         

            }
            catch (FileNotFoundException e)
            {
                //Response.Write(e.Message);
               
                return Content("false");
            }
            catch (HttpException e)
            {
                //Response.Write(e.Message);
               
                return Content("false");
            }
            finally
            {
                if (myImg != null)
                    myImg.Dispose();
            }
            imgName = filename + ".png";
            return Content("/Images/web/upload/" + imgName);

        }
    }
}