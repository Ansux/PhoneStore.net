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
using System.Data.Entity.Validation;
using System.Net.Mime;

namespace PhoneStoreSystem.Controllers
{
    public class ProductInfoController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        private static List<ProductStorageAndStyle> ProductStorages = new List<ProductStorageAndStyle>();
        private static int psId = 0;

        private static string imgName = "";
        //
        // GET: /ProductInfo/

        public ActionResult Index(string ProductTypes = null, string ProAuthors = null, string ProName = null, string ProIsSales=null, string SaleDateStart = null, string SaleDateEnd = null)
        {
            var productinfos = db.ProductInfos.Include(p => p.ProductType);
          
            //ProAuthors = Session["UserName"].ToString();
            
            ViewBag.ProAuthors = new SelectList(db.UserInfos, "UserName", "UserName", ProAuthors);
            ViewBag.ProductTypes = new SelectList(db.ProductTypes, "TypeId", "TypeName", ProductTypes);
          
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem { Text = "是", Value = "1" },
                new SelectListItem { Text = "否", Value = "0" } 
            };
            ViewBag.ProIsSales = new SelectList(list,"Value","Text",ProIsSales);

            ViewBag.proName = ProName;
            ViewBag.proAuthor = ProAuthors;
            ViewBag.typeId = ProductTypes;
            ViewBag.proIsSale = ProIsSales;
            ViewBag.startDate = SaleDateStart;
            ViewBag.endDate = SaleDateEnd;

            if (!String.IsNullOrEmpty(ProIsSales))
            { 
                bool isSale=ProIsSales=="0"?false:true;
                productinfos = productinfos.Where(i => i.ProIsSale == isSale);
                ViewBag.pageIndex = 0;
            }


            if (!String.IsNullOrEmpty(SaleDateStart) && !String.IsNullOrEmpty(SaleDateEnd))
            {
                DateTime startDate = Convert.ToDateTime(SaleDateStart);
                DateTime endDate = Convert.ToDateTime(SaleDateEnd).AddDays(1);
                productinfos = productinfos.Where(i => i.ProSaleDate >= startDate).Where(i => i.ProSaleDate <= endDate);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProName))
            {
                productinfos = productinfos.Where(i => i.ProName.Contains(ProName));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProAuthors))
            {
                productinfos = productinfos.Where(i => i.ProAuthor.Contains(ProAuthors));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductTypes))
            {
                int tid = int.Parse(ProductTypes);
                productinfos = productinfos.Where(i => i.TypeId == tid);
                ViewBag.pageIndex = 0;
            }

            ViewBag.listCount = productinfos.ToList().Count();
            
            return View();
        }


        public ActionResult GetPageList(int pageIndex = 0, int pageSize = 10, string ProductTypes = null, string ProAuthors = null, string ProName = null,string ProIsSales=null, string SaleDateStart = null, string SaleDateEnd = null)
        {
            //默认采用创建代理的方式返回实体集合，这里必须要关闭才可以读出数据
            db.Configuration.ProxyCreationEnabled = false;

            var productInfos = from n in db.ProductInfos select n;
            
            ViewBag.ProAuthors = new SelectList(db.UserInfos, "UserName", "UserName", ProAuthors);
            ViewBag.ProductTypes = new SelectList(db.ProductTypes, "TypeId", "TypeName", ProductTypes);


            if (!String.IsNullOrEmpty(ProIsSales))
            {
                bool isSale = ProIsSales == "0" ? false : true;
                productInfos = productInfos.Where(i => i.ProIsSale == isSale);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(SaleDateStart) && !String.IsNullOrEmpty(SaleDateEnd))
            {
                DateTime startDate = Convert.ToDateTime(SaleDateStart);
                DateTime endDate = Convert.ToDateTime(SaleDateEnd).AddDays(1);
                productInfos = productInfos.Where(i => i.ProSaleDate >= startDate).Where(i => i.ProSaleDate <= endDate);
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProName))
            {
                productInfos = productInfos.Where(i => i.ProName.Contains(ProName));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProAuthors))
            {
                productInfos = productInfos.Where(i => i.ProAuthor.Contains(ProAuthors));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(ProductTypes))
            {
                int tid = int.Parse(ProductTypes);
                productInfos = productInfos.Where(i => i.TypeId == tid);
                ViewBag.pageIndex = 0;
            }

            //var productinfos = db.ProductInfos.Include(p => p.Pstyle);
            return Json(productInfos.OrderByDescending(i => i.ProSaleDate).ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);
        
        }
        //
        // GET: /ProductInfo/Details/5

        public ActionResult Details(int id = 0)
        {
            ProductStorageAndStyle ps;
            List<ProductStorage> p = db.ProductStorages.Where(a => a.ProId == id).ToList();

            ProductInfo productinfo = db.ProductInfos.Find(id);

            if (productinfo == null)
            {
                throw new HttpException(404, "page not found");
            }

            ProductStorages.Clear();

            try
            {
                foreach (var x in p)
                {
                    ps = new ProductStorageAndStyle()
                    {
                        id = x.StorageId,
                        ProImage = x.ProImage,
                        ColorId = x.ColorId,
                        ColorName = x.ProductColor.ColorName,
                        StorageNumber = x.StorageNumber
                    };
                    psId = x.StorageId;
                    ProductStorages.Add(ps);
                }
            }
            catch (Exception e)
            {

            }
            ViewBag.ImagUrl = productinfo.ProCoverImage;

            return View(productinfo);
        }

        //
        // GET: /ProductInfo/Create

        public ActionResult Create()
        {
            //每次重新进入该界面，就要清空它。
            ProductStorages.Clear();
            psId = 0;

            imgName = "";
            ViewBag.ImagUrl = "../Images/web/white.gif";

            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.TypeId = new SelectList(db.ProductTypes, "TypeId", "TypeName");
            return View();

        }

        //
        // POST: /ProductInfo/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] //因为：检测到有潜在危险的 Request.Form 值。
        public ActionResult Create(ProductInfo productinfo)
        {            
            ViewBag.ImagUrl = "../Images/web/white.gif";
            if (imgName == "")
            {

                ViewBag.ErrorMessage = "创建失败，请先点击上传图片！";
                ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
                ViewBag.TypeId = new SelectList(db.ProductTypes, "TypeId", "TypeName",productinfo.TypeId);        
                return View(productinfo);
            }
            else
            {
                ViewBag.ImagUrl = imgName;
            }

            try
            {
                //if (ModelState.IsValid)
                //{
                    //将其他未输入的值，在这里赋值。
                    productinfo.ProCoverImage = imgName;
             
                    db.ProductInfos.Add(productinfo);
                    db.SaveChanges();
                    imgName = "";


                    int proId = db.ProductInfos.OrderByDescending(i => i.ProId).First().ProId;
                    ProductStorage ps = null;
                    foreach (var p in ProductStorages) {
                        ps=new ProductStorage();
                        ps.ColorId = p.ColorId;
                        ps.ProImage = p.ProImage;
                        ps.StorageNumber = p.StorageNumber;
                        ps.ProId = proId;                       

                        db.ProductStorages.Add(ps);
                    }
                    db.SaveChanges();
            
                    return RedirectToAction("Index");
                //}
            }
            catch (DbEntityValidationException e)
            {
                ViewBag.ErrorMessage = "您的数据有误，请检查！";                
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "您的数据有误，请检查！";               
            }

            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.TypeId = new SelectList(db.ProductTypes, "TypeId", "TypeName",productinfo.TypeId);
            return View("Create", productinfo);
        }

        //
        // GET: /ProductInfo/Edit/5

        public ActionResult Edit(int id = 0)
        {
            //ProductStorageAndStyle ps;
            //List<ProductStorage> p= db.ProductStorages.Where(a => a.ProId == id).ToList();
            ProductInfo productinfo = db.ProductInfos.Find(id);
            
            if (productinfo == null)
            {
                throw new HttpException(404, "page not found");
            }

            //ProductStorages.Clear();
            /*
            try
            {
                foreach (var x in p)
                {
                    ps = new ProductStorageAndStyle()
                    {
                        id = x.StorageId,
                        ProImage = x.ProImage,
                        ColorId = x.ColorId,
                        ColorName = x.ProductColor.ColorName,
                        StorageNumber = x.StorageNumber
                    };
                    psId = x.StorageId;
                    ProductStorages.Add(ps);   
                }
            }
            catch (Exception e) { 
            
            }*/

            imgName = "";
            ViewBag.ImagUrl = productinfo.ProCoverImage;
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.TypeId = new SelectList(db.ProductTypes, "TypeId", "TypeName", productinfo.TypeId);
            return View(productinfo);
        }

        //
        // POST: /ProductInfo/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] //因为：检测到有潜在危险的 Request.Form 值。
        public ActionResult Edit(ProductInfo productinfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imgName != "")
                        //将其他未输入的值，在这里赋值。
                        productinfo.ProCoverImage = imgName;
                    db.Entry(productinfo).State = EntityState.Modified;
                    db.SaveChanges();
                    imgName = "";

                    int proId = productinfo.ProId;
                    /*
                    ProductStorageAndStyle psa = null;
                    List<ProductStorage> p = db.ProductStorages.Where(a => a.ProId == proId).ToList();
                    try
                    {
                        //从表中删除已经移除了的数据
                        foreach (var x in p)
                        {
                            int id = x.StorageId;
                            psa = ProductStorages.Find(i => i.id == id);
                            if (psa == null)
                            {
                                db.ProductStorages.Remove(x);
                            }
                        }
                       // db.SaveChanges();

                        //添加新的数据
                        ProductStorage ps = null;
                        foreach (var p1 in ProductStorages)
                        {
                            ps = new ProductStorage();
                            ps.ColorId = p1.ColorId;
                            ps.ProImage = p1.ProImage;
                            ps.StorageNumber = p1.StorageNumber;
                            ps.ProId = proId;

                            db.ProductStorages.Add(ps);
                        }
                        db.SaveChanges();  
                    }
                    catch (Exception e)
                    {

                    }
                    */
                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException e)
            {
                ViewBag.ErrorMessage = "您的数据有误，请检查！";               
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "您的数据有误，请检查！";               
            }

            if (imgName != "")
            {
                ViewBag.ProductImage = imgName;
            } 
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.TypeId = new SelectList(db.ProductTypes, "TypeId", "TypeName");
            return View(productinfo);
        }

        //用于检测是否存在
        public JsonResult IsExistProName(string proName)
        {
            bool isExist = true;
            try
            {
                string[] url = Request.UrlReferrer.LocalPath.Split('/');
                string actionName = url[2];
                if (actionName == "Create")
                {
                    var u = from n in db.ProductInfos
                            where n.ProName == proName
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
                else if (actionName == "Edit")
                {
                    int id = int.Parse(url[url.Length - 1]);

                    var u = from n in db.ProductInfos
                            where n.ProName == proName && n.ProId != id
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
            }
            catch
            {
                var u = from n in db.ProductInfos
                        where n.ProName == proName
                        select n; ;
                if (u.Count() != 0)
                {
                    isExist = false;
                }
            }

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ProductInfo/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ProductStorageAndStyle ps;
            List<ProductStorage> p = db.ProductStorages.Where(a => a.ProId == id).ToList();

            ProductInfo productinfo = db.ProductInfos.Find(id);

            if (productinfo == null)
            {
                throw new HttpException(404, "page not found");
            }

            ProductStorages.Clear();

            try
            {
                foreach (var x in p)
                {
                    ps = new ProductStorageAndStyle()
                    {
                        id = x.StorageId,
                        ProImage = x.ProImage,
                        ColorId = x.ColorId,
                        ColorName = x.ProductColor.ColorName,
                        StorageNumber = x.StorageNumber
                    };
                    psId = x.StorageId;
                    ProductStorages.Add(ps);
                }
            }
            catch (Exception e)
            {

            }
            ViewBag.ImagUrl = productinfo.ProCoverImage;

            return View(productinfo);
        }

        //
        // POST: /ProductInfo/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*
            List<ProductStorage> p = db.ProductStorages.Where(a => a.ProId == id).ToList();
            try
            {

                //从表中删除已经移除了的数据
                foreach (var x in p)
                {       
                    db.ProductStorages.Remove(x);      
                }

                ProductInfo productinfo = db.ProductInfos.Find(id);
                db.ProductInfos.Remove(productinfo);

                db.SaveChanges();
                
            }
            catch (Exception e) { 
                
            }*/

            ProductInfo productinfo = db.ProductInfos.Find(id);
            productinfo.ProIsSale = false;

            var s=db.Carts.Where(c => c.ProId == id);
            foreach (var c in s) {
                db.Carts.Remove(c);
            }
            
            db.SaveChanges();

            return RedirectToAction("Index");
           
        }
              
        public ActionResult Reshelf(int id)
        {           

            ProductInfo productinfo = db.ProductInfos.Find(id);
            productinfo.ProIsSale = true;

            db.SaveChanges();

            return RedirectToAction("Index");
           
        }
        

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult GetProductStorages() {
            return  Json(ProductStorages,JsonRequestBehavior.AllowGet);
        }

        public ActionResult removeProductStorage(int id)
        {
            ProductStorageAndStyle ps = ProductStorages.SingleOrDefault(i=>i.id==id);
            ProductStorages.Remove(ps);
            return Content("OK");
        }
        
        //添加款式
        public ActionResult AddStyle_FileImg()
        {

            int cId = int.Parse(Request.Form["ColorId"] as string);
            foreach (var s in ProductStorages) {
                if (s.ColorId == cId) {
                    return Content("false_ColorError");
                }
            }


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
                    Bitmap s = PublicController.SizeImage(myImg, 500, 500);
                    PublicController.SaveFormatJpeg(s, Server.MapPath("../Images/web/productImg/" + filename + ".jpeg"),100L);
                    s.Dispose();*/

                    Bitmap s = PublicController.SizeImage(myImg, 500, 500);

                    s.Save(Server.MapPath("../Images/web/storageImg/" + filename + ".png"));

                    s.Dispose();
                }
                else {
                    throw new HttpException();
                }

            }
            catch (FileNotFoundException e)
            {
                //Response.Write(e.Message);
                //ViewBag.newsInfoCreateError = "上传失败，请联系开发员！";
                return Content("false_ExceptionError");
            }
            catch (HttpException e)
            {
                //Response.Write(e.Message);
                //ViewBag.newsInfoCreateError = "上传失败，图片质量过大！";
                return Content("false_ExceptionError");
            }
            finally
            {
                if (myImg != null)
                    myImg.Dispose();
            }

            
            
            ProductStorageAndStyle ps = new ProductStorageAndStyle()
            {
                id=++psId,
                ProImage = filename + ".png",
                ColorId = cId,
                StorageNumber = int.Parse(Request.Form["StorageNumber"] as string),
                ColorName = db.ProductColors.Where(i=>i.ColorId==cId).First().ColorName
            };
            ProductStorages.Add(ps);
            return Content("true");

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


                    //Bitmap s = PublicController.SizeImage(myImg, 240, 320);
                    //PublicController.SaveFormatJpeg(s, Server.MapPath("../Images/web/productImg/" + filename + ".jpeg"), 70L);
                    //s.Dispose();

                    Bitmap s = PublicController.SizeImage(myImg, 500, 500);

                    s.Save(Server.MapPath("../Images/web/productImg/" + filename + ".png"));

                    s.Dispose();
                }
                else {
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