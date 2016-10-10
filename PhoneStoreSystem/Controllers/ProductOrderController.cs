using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using PhoneStoreSystem.Filters;

namespace PhoneStoreSystem.Controllers
{
    [VaildateLoginRole(Role = ("Administrator,Manager"))]
    [OrderStatusAutoValidate]  
    public class ProductOrderController : Controller
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        //
        // GET: /ProductOrder/

        public ActionResult Index(string OrderStatus = null, string OrderNumber = null/*, string OrderDateStart = null, string OrderDateEnd = null*/)
        {
            var orders = db.Orders.Include(o => o.OrderStatus).Include(o => o.Member);

            ViewBag.OrderStatus = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusIdName", OrderStatus);

            ViewBag.orderNumber = OrderNumber;
            ViewBag.statusId = OrderStatus;
            //ViewBag.startDate = OrderDateStart;
            //ViewBag.endDate = OrderDateEnd;
            /*
            if (!String.IsNullOrEmpty(OrderDateStart) && !String.IsNullOrEmpty(OrderDateEnd))
            {
                //DateTime startDate = Convert.ToDateTime(OrderDateStart);
                //DateTime endDate = Convert.ToDateTime(OrderDateEnd);
                //orders = orders.Where(i => i.OrderPayTime.ToString() as DateTime >= startDate).Where(i => i.ProSaleDate <= endDate);
                ViewBag.pageIndex = 0;
            }*/
            if (!String.IsNullOrEmpty(OrderNumber))
            {
                orders = orders.Where(i => i.OrderNumber.Contains(OrderNumber));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(OrderStatus))
            {
                int sid = int.Parse(OrderStatus);
                orders = orders.Where(i => i.OrderStatusId == sid);
                ViewBag.pageIndex = 0;
            }

            ViewBag.listCount = orders.ToList().Count();
        
            return View();
        }

        public ActionResult GetPageList(int pageIndex = 0, int pageSize = 10, string OrderStatus = null, string OrderNumber = null/*, string OrderDateStart = null, string OrderDateEnd = null*/)
        {
            //默认采用创建代理的方式返回实体集合，这里必须要关闭才可以读出数据
            db.Configuration.ProxyCreationEnabled = false;

            var orders = db.Orders.Include(o => o.OrderStatus).Include(o => o.Member);

            /*
            if (!String.IsNullOrEmpty(OrderDateStart) && !String.IsNullOrEmpty(OrderDateEnd))
            {
                //DateTime startDate = Convert.ToDateTime(OrderDateStart);
                //DateTime endDate = Convert.ToDateTime(OrderDateEnd);
                //orders = orders.Where(i => i.OrderPayTime.ToString() as DateTime >= startDate).Where(i => i.ProSaleDate <= endDate);
                ViewBag.pageIndex = 0;
            }*/
            if (!String.IsNullOrEmpty(OrderNumber))
            {
                orders = orders.Where(i => i.OrderNumber.Contains(OrderNumber));
                ViewBag.pageIndex = 0;
            }
            if (!String.IsNullOrEmpty(OrderStatus))
            {
                int sid = int.Parse(OrderStatus);
                orders = orders.Where(i => i.OrderStatusId == sid);
                ViewBag.pageIndex = 0;
            }

            //var productinfos = db.ProductInfos.Include(p => p.Pstyle);
            return Json(orders.OrderByDescending(i => i.OrderId).ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /ProductOrder/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                throw new HttpException(404, "page not found");
            }
            ViewBag.ImagUrl = order.OrderProCoverImage;
            return View(order);
        }

        public ActionResult Send(int id) 
        {

            Order order = db.Orders.Find(id);
            order.OrderStatusId = 3;
            order.OrderSendTime = DateTime.Now.ToString();
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AllowReturn(int id)
        {
            Order order = db.Orders.Find(id);
            order.OrderStatusId = 7;

            var storage=db.ProductStorages.Find(order.StorageId);
            storage.StorageNumber = order.OrderCount;

            var pro = db.ProductInfos.Find(order.ProId);
            pro.ProSales = pro.ProSales - order.OrderCount;
            db.Entry(pro).State = EntityState.Modified;

            var payment = db.Payments.Find(order.Member.PaymentId);
            payment.PaymentBalance =payment.PaymentBalance + (order.OrderCount * order.OrderPrice);

            PaymentLog pl = new PaymentLog();
            pl.PaymentId = payment.PaymentId;
            pl.PaymentLogName = "退货成功："+order.OrderProName+" "+order.ColorName;
            pl.PaymentLogPrice = order.OrderCount * order.OrderPrice;
            pl.PaymentLogTime = DateTime.Now;

            db.PaymentLog.Add(pl);
            
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}