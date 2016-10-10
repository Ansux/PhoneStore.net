using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using System.Collections;

namespace PhoneStoreSystem.Controllers
{
    public class WebPartialController : Controller
    {
        PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /WebPartial/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _NavPartial() {

            return PartialView(db.Navigations.Take(5).ToList());
        }
        public ActionResult _sideNavPartial()
        {
           var type = db.ProductTypes.Take(7).ToList();
           ArrayList al=new ArrayList();
           IList list;
           foreach (var x in type)
           {
               list = new ArrayList();
               var proInfos = x.ProductInfos.Where(p => p.TypeId == x.TypeId && p.ProIsSale==true).OrderByDescending(p => p.ProSales).Select(i => new { i.ProName, i.ProId }).Take(3).ToList();
               ArrayList plist = new ArrayList();
               foreach(var pi in proInfos){
                   plist.Add(pi.ProId+";"+pi.ProName);
               }
               list.Add(x.TypeName);
               list.Add(plist);
               al.Add(list);
           }

           ViewBag.ProList = al;

           return PartialView();
        }
        
        public ActionResult _MemberCenterPartial() {
            return PartialView();
        }

        public ActionResult _mainHotChoose(int row=-1)
        {
            return PartialView(db.ProductInfos.Where(p=>p.ProIsSale==true).OrderByDescending(o => o.ProSaleDate).Take(row).OrderByDescending(o => o.ProReadNumber));        
        }

        public ActionResult _SlidePartial(int row = -1)
        {
            return PartialView(db.Slides.Take(row).ToList());
        }

        public ActionResult _mainHotProduct(int row = -1)
        {
            var proType = db.ProductTypes.OrderBy(t=>t.TypeId).Take(4).ToList();
            ArrayList al = new ArrayList();
            
            foreach (var x in proType)
            {              
                var p = db.ProductInfos.Where(i => i.TypeId == x.TypeId&&i.ProIsSale==true).OrderByDescending(o => o.ProReadNumber).Take(6);
                al.Add(p);                 
            }
           
            ViewBag.ProType = proType;
            ViewBag.ProInfo = al;
            return PartialView(db.ProductInfos.Where(p=>p.ProIsSale==true).OrderByDescending(o => o.ProReadNumber).Take(row));
        }

        public ActionResult _mainNewProduct(int row = -1)
        {
            var proType = db.ProductTypes.OrderBy(t => t.TypeId).Take(4).ToList();
            ArrayList al = new ArrayList();
            foreach (var x in proType)
            {
                var p = db.ProductInfos.Where(i => i.TypeId == x.TypeId && i.ProIsSale == true).OrderByDescending(o => o.ProSaleDate).Take(row);
                al.Add(p);
            }

            ViewBag.ProType = proType;
            ViewBag.ProInfo = al;
 
            return PartialView(db.ProductInfos.Where(p=>p.ProIsSale==true).OrderByDescending(o => o.ProSaleDate).Take(row));
            
        }
        
    }
}
