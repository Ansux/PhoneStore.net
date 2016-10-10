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
    [VaildateLoginRole(Role = ("Administrator,Manager"))]
    [OrderStatusAutoValidate]  
    public class ProductStorageController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();
        private static string imgName = "";
        //
        // GET: /ProductStorage/

        public ActionResult Index(string ProductTypes = null, string ProductColors = null, string ProName = null)
        {
            var productstorages = db.ProductStorages.Include(p => p.ProductInfo).Include(p => p.ProductColor);
          
            ViewBag.ProductTypes = new SelectList(db.ProductTypes, "TypeId", "TypeName", ProductTypes);
            ViewBag.ProductColors = new SelectList(db.ProductColors, "ColorId", "ColorName", ProductColors);
      
            ViewBag.ProName = ProName;
            ViewBag.colorId = ProductColors;
            ViewBag.typeId = ProductTypes;         
           
            if (!String.IsNullOrEmpty(ProName))
            {
                productstorages = productstorages.Where(i => i.ProductInfo.ProName.Contains(ProName));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductColors))
            {
                int cid = int.Parse(ProductColors);
                productstorages = productstorages.Where(i => i.ColorId == cid);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductTypes))
            {
                int tid = int.Parse(ProductTypes);
                productstorages = productstorages.Where(i => i.ProductInfo.TypeId == tid);
                ViewBag.pageIndex = 0;
            }

           
            ViewBag.listCount = productstorages.ToList().Count();    
            return View();
        }

        public ActionResult GetPageList(int pageIndex = 0, int pageSize = 10, string ProductTypes = null, string ProductColors = null, string ProName = null)
        {
            //默认采用创建代理的方式返回实体集合，这里必须要关闭才可以读出数据
            db.Configuration.ProxyCreationEnabled = false;

            var productstorages = db.ProductStorages.Include(p => p.ProductInfo).Include(p => p.ProductColor);

            if (!String.IsNullOrEmpty(ProName))
            {
                productstorages = productstorages.Where(i => i.ProductInfo.ProName.Contains(ProName));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductColors))
            {
                int tid = int.Parse(ProductColors);
                productstorages = productstorages.Where(i => i.ColorId == tid);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductTypes))
            {
                int tid = int.Parse(ProductTypes);
                productstorages = productstorages.Where(i => i.ProductInfo.TypeId == tid);
                ViewBag.pageIndex = 0;
            }

            //var productinfos = db.ProductInfos.Include(p => p.Pstyle);
            return Json(productstorages.OrderByDescending(i => i.StorageId).ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);

        }


        //
        // GET: /ProductStorage/Details/5

        public ActionResult Details(int id = 0)
        {
            ProductStorage productstorage = db.ProductStorages.Find(id);
            if (productstorage == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(productstorage);
        }

        //
        // GET: /ProductStorage/Create

        public ActionResult Create()
        {
            imgName = "";
            ViewBag.ProductImage = "../Images/web/white.gif";
            ViewBag.ProId = new SelectList(db.ProductInfos, "ProId", "ProName");
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            return View();
        }

        //
        // POST: /ProductStorage/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductStorage productstorage)
        {
            ViewBag.ProductImage = "../Images/web/white.gif";
            if (imgName == "")
            {

                ViewBag.ErrorMessage = "创建失败，请先点击上传图片！";
                ViewBag.ProId = new SelectList(db.ProductInfos, "ProId", "ProName", productstorage.ProId);
                ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productstorage.ColorId);

                return View(productstorage);
            }
            else {
                ViewBag.ProductImage = imgName;
            }

            List<int> cIds = db.ProductStorages.Where(i=>i.ProId==productstorage.ProId).Select(i=>i.ColorId).ToList();
            int cId = productstorage.ColorId;
            if (cIds.Contains(cId)) {
                ViewBag.ErrorMessage = "创建失败，已存在该颜色！";
                ViewBag.ProId = new SelectList(db.ProductInfos, "ProId", "ProName", productstorage.ProId);
                ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productstorage.ColorId);   
                
                return View(productstorage);
            }            

            if (ModelState.IsValid)
            {
                productstorage.ProImage = imgName;
                db.ProductStorages.Add(productstorage);
                db.SaveChanges();
                imgName = "";
                return RedirectToAction("Index");
            }  
            ViewBag.ProId = new SelectList(db.ProductInfos, "ProId", "ProName", productstorage.ProId);
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productstorage.ColorId);
            return View(productstorage);
        }

        //
        // GET: /ProductStorage/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProductStorage productstorage = db.ProductStorages.Find(id);
            if (productstorage == null)
            {
                throw new HttpException(404, "page not found");
            }
            imgName = "";
            ViewBag.ProductImage = productstorage.ProImage;
            return View(productstorage);
        }

        //
        // POST: /ProductStorage/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductStorage productstorage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imgName != "")
                        //将其他未输入的值，在这里赋值。
                        productstorage.ProImage = imgName;
                    db.Entry(productstorage).State = EntityState.Modified;
                    db.SaveChanges();
                    imgName = "";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e) { 
                
            }

            if(imgName != ""){
                ViewBag.ProductImage = imgName;
            } 
            return View(productstorage);
        }

        //
        // GET: /ProductStorage/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ProductStorage productstorage = db.ProductStorages.Find(id);
            if (productstorage == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(productstorage);
        }

        //
        // POST: /ProductStorage/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductStorage productstorage = db.ProductStorages.Find(id);
            db.ProductStorages.Remove(productstorage);
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


                    Bitmap s = PublicController.SizeImage(myImg, 500, 500);
                    s.Save(Server.MapPath("../Images/web/storageImg/" + filename + ".png"));
                    //PublicController.SaveFormatJpeg(s, Server.MapPath("../Images/web/storageImg/" + filename + ".jpeg"), 70L);
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