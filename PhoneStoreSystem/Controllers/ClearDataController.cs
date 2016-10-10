using PhoneStoreSystem.Filters;
using PhoneStoreSystem.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneStoreSystem.Controllers
{
    [VaildateLoginRole(Role = ("Administrator"))]  
    public class ClearDataController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /ClearData/

        public ActionResult Index()
        {
            string uploadPath = Server.MapPath("~\\Images\\web\\upload"); //临时文件夹路径
            string productPath = Server.MapPath("~\\Images\\web\\productImg"); //产品图片文件夹路径
            string storagePath = Server.MapPath("~\\Images\\web\\storageImg"); //产品图片文件夹路径
            string memberPath = Server.MapPath("~\\Images\\web\\memberImg"); //会员相片文件夹路径
            string slidePath = Server.MapPath("~\\Images\\web\\slideImg"); //幻灯片文件夹路径

            string[] uploadFiles = Directory.GetFiles(uploadPath);
            string[] productFiles = Directory.GetFiles(productPath);
            string[] storageFiles = Directory.GetFiles(storagePath);
            string[] memberFiles = Directory.GetFiles(memberPath);
            string[] slidesFiles = Directory.GetFiles(slidePath);

            ArrayList uploadList = new ArrayList(uploadFiles);
            ArrayList productList = new ArrayList(productFiles);
            ArrayList storageList = new ArrayList(storageFiles);
            ArrayList memberList = new ArrayList(memberFiles);
            ArrayList slidesList = new ArrayList(slidesFiles);

            ViewBag.UploadCount = uploadList.Count;
            ViewBag.ProductCount = productList.Count;
            ViewBag.StorageCount = storageList.Count;
            ViewBag.MemberCount = memberList.Count;
            ViewBag.SlidesCount = slidesList.Count;
            return View();
        }

        
        [HttpPost]
        [ActionName("index")]
        public ActionResult Indexs()
        {

            string uploadPath = Server.MapPath("~\\Images\\web\\upload"); //临时文件夹路径
            string productPath = Server.MapPath("~\\Images\\web\\productImg"); //产品图片文件夹路径
            string storagePath = Server.MapPath("~\\Images\\web\\storageImg"); //产品款式图片文件夹路径
            string memberPath = Server.MapPath("~\\Images\\web\\memberImg"); //会员相片文件夹路径
            string slidePath = Server.MapPath("~\\Images\\web\\slideImg"); //幻灯片文件夹路径

            string[] uploadFiles = Directory.GetFiles(uploadPath);
            string[] productFiles = Directory.GetFiles(productPath);
            string[] storageFiles = Directory.GetFiles(storagePath);
            string[] memberFiles = Directory.GetFiles(memberPath);
            string[] slidesFiles = Directory.GetFiles(slidePath);
            
            ArrayList uploadList = new ArrayList(uploadFiles);
            ArrayList productList = new ArrayList(productFiles);
            ArrayList storageList = new ArrayList(storageFiles);
            ArrayList memberList = new ArrayList(memberFiles);
            ArrayList slidesList = new ArrayList(slidesFiles);
           
            ArrayList productsDataList = new ArrayList(db.ProductInfos.Select(z => z.ProCoverImage).ToArray());
            ArrayList storagesDataList = new ArrayList(db.ProductStorages.Select(z => z.ProImage).ToArray());
            ArrayList membersDataList = new ArrayList(db.Members.Select(z => z.MemberPhoto).ToArray());
            ArrayList slidesDataList = new ArrayList(db.Slides.Select(s => s.SidImgUrl).ToArray());

            try
            {
                foreach (string i in uploadList)
                {
                    //fs = System.IO.File.Delete(System.Environment.CurrentDirectory + "\\updateFile\\Test_Update.exe");
                    FileInfo up = new FileInfo(i.ToString());
                    if (up.Exists)
                    {
                        try
                        {
                            up.Delete();
                        }
                        catch
                        {
                            ViewBag.dataMessage = "暂时无法清理数据，可能有文件被占用，请稍后执行！！";
                            return View();
                        }
                    }

                }

                for (int i = 0; i < productList.Count; i++)
                {
                    FileInfo proFile = new FileInfo(productList[i].ToString());

                    string s = proFile.Name;
                    if (!productsDataList.Contains(s))
                    {
                        if (proFile.Exists)
                            proFile.Delete();
                    }

                }
                for (int i = 0; i < storageList.Count; i++)
                {
                    FileInfo stoFile = new FileInfo(storageList[i].ToString());

                    string s = stoFile.Name;
                    if (!storagesDataList.Contains(s))
                    {
                        if (stoFile.Exists)
                            stoFile.Delete();
                    }

                }

                for (int i = 0; i < memberList.Count; i++)
                {
                    FileInfo memberFile = new FileInfo(memberList[i].ToString());

                    string s = memberFile.Name;
                    if (!membersDataList.Contains(s))
                    {
                        if (memberFile.Exists)
                            memberFile.Delete();
                    }
                }


                for (int i = 0; i < slidesList.Count; i++)
                {
                    FileInfo slidesFile = new FileInfo(slidesList[i].ToString());

                    string s = slidesFile.Name;
                    if (!slidesDataList.Contains(s))
                    {
                        if (slidesFile.Exists)
                            slidesFile.Delete();
                    }
                }

                ViewBag.dataMessage = "清理成功！！";
            }
            catch (Exception e)
            {
                ViewBag.dataMessage = "暂时无法清理数据，可能有文件被占用，请稍后执行！！";

            }
            finally
            {


            }
            return RedirectToAction("Index");

        }
        
    }
}
