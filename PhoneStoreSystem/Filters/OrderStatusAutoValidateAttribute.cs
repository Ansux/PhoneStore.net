using PhoneStoreSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneStoreSystem.Filters
{
    public class OrderStatusAutoValidateAttribute : ActionFilterAttribute
    {
        PhoneStoreContext db = new PhoneStoreContext();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //1.处理未支付的
            //2.处理发货后而未确认收货的     
            if (db != null)
            {
                var orders1 = db.Orders.Where(o => o.OrderStatusId == 1);
                var orders3 = db.Orders.Where(o => o.OrderStatusId == 3);

                foreach (var o in orders1)
                {
                    if (DateTime.Now > DateTime.Parse(o.OrderTime).AddHours(24))
                    {
                        o.OrderStatusId = 5;
                    }
                }

                foreach (var oo in orders3)
                {
                    if (DateTime.Now > DateTime.Parse(oo.OrderTime).AddDays(7))
                    {
                        oo.OrderSuccessTime = DateTime.Now.ToString();
                        oo.OrderStatusId = 4;

                        var pro = db.ProductInfos.Find(oo.ProId);
                        pro.ProSales = pro.ProSales + oo.OrderCount;
                       
                    }
                }
                db.SaveChanges();
            }
        }
    }    
}