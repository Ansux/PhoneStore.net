using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using NewsSystem.Controllers;
using System.Data;
using System.Drawing;
using System.IO;
using PhoneStoreSystem.Filters;

namespace PhoneStoreSystem.Controllers
{
    [ValidateMemberLogin]
    public class WebPublicController : Controller
    {
        //
        // GET: /WebPublic/
        PhoneStoreContext db = new PhoneStoreContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCart(int proId = -1, int proStorageId = -1, int cartCount = 0, int proColorId = 1)
        {
            var memberId = int.Parse(Session["MemberId"].ToString());
            CheckCart(memberId);

            try
            {
                
                var cart = db.Carts.SingleOrDefault(c => c.StorageId == proStorageId && c.MemberId == memberId);//如果已存在同款手机，就加数量
                if (cart != null)
                {
                    cart.CartTime = DateTime.Now;
                    cart.CartCount = cart.CartCount + cartCount;
                    if (cart.CartCount>10)
                         return Content("单件商品不允许超过10件！");
                }
                else
                {
                    var ccc = db.Carts.Where(i => i.MemberId == memberId);
                    if (ccc.Count() == 5)
                    {
                        return Content("购物车已满了，赶紧去清理吧！"); 
                    }
                    Cart c = new Cart();
                    c.CartTime = DateTime.Now;
                    c.CartCount = cartCount;                                   
                    c.MemberId = int.Parse(Session["MemberId"].ToString());
                    c.ProId = proId;
                    c.ColorId = proColorId;
                    c.StorageId = proStorageId;

                    db.Carts.Add(c);
                    db.Entry(c).State = EntityState.Added;
                }
                db.SaveChanges();
            }
            catch (Exception) {
                return Content("系统异常，请刷新！"); 
            }
            return Content("True");
        }

        public ActionResult RemoveCart(int cartId=-1)
        {
            try {                
                db.Carts.Remove(db.Carts.Find(cartId));
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("系统异常，请刷新！");
            }

            return Content("True");
        }

        public ActionResult ChangeCartCount(int cartId, int cartCount)
        {
            try
            {
                var x=db.Carts.Find(cartId);
                x.CartCount = cartCount;
                db.Entry(x).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("系统异常，请稍后尝试！");
            }

            return Content("True");
        }
        public ActionResult SubmitCart()
        {
            string orderIds = "";
            string pro = Request.Form["ProInfo"];
            string cartsInfo = Request.Form["CartsInfo"];

            if (cartsInfo != "" && cartsInfo!=null)
            {
                string checkResult = CheckCartNum(cartsInfo);
                if (checkResult == "DoubleSubmit")
                {
                    return Content("DoubleSubmit");
                }
                else if (checkResult=="NotStorage")
                {
                    return Content("NotStorage");
                }
            }           
           
            try
            {
                int memberId = int.Parse(Session["MemberId"].ToString());
                    
                int addressId = int.Parse(Request.Form["AddressId"].ToString());            
                   
                Order order = null;

                if (cartsInfo != "" && cartsInfo != null)
                {
                    string[] carts = cartsInfo.Split(',');
                    for (int i = 0; i < carts.Length - 1; i++)
                    {
                        int cartId = int.Parse(carts[i]);
                        var cart = db.Carts.Find(cartId);
                        if (cart == null)
                        {
                            return Content("DoubleSubmit");
                        }
                        var proSorage = db.ProductStorages.Find(cart.StorageId);
                        var proInfo = cart.ProductInfo;
                        var proColor = cart.ProductColor;
                        if (cart.CartCount > proSorage.StorageNumber)
                        {
                            return Content("NotStorage");
                        }
                        proSorage.StorageNumber = proSorage.StorageNumber - cart.CartCount;

                        order = new Order();

                        //赵志康 158****2144   辽宁  本溪市  平山区  阿萨德阿萨德按时
                        var address = db.Addresses.SingleOrDefault(a => a.AddressId == addressId);
                        order.AddressDetails = address.AddressName + " " + address.AddressPhone + " " +
                            address.AddressProvince + " " + address.AddressCity + " " + address.AddressCounty + " " + address.AddressDetail;

                        long time = PublicController.TimeStamp();
                        orderIds += time + "；";
                        order.OrderNumber = Convert.ToString(time);

                        order.MemberId = memberId;
                        order.OrderCount = cart.CartCount;
                        order.ColorName = proColor.ColorName;
                        order.OrderPrice = proInfo.ProPrice;
                        order.OrderProCoverImage = proInfo.ProCoverImage;
                        order.OrderProIntroduce = proInfo.ProFeature;
                        order.OrderProName = proInfo.ProName;
                        order.OrderStatusId = 1;
                        order.OrderTime = DateTime.Now.ToString();
                        order.ProId = proInfo.ProId;
                        order.StorageId = cart.StorageId;

                        db.Orders.Add(order);
                        db.Entry(order).State = EntityState.Added;
                        db.Carts.Remove(cart);
                        db.Entry(cart).State = EntityState.Deleted;
                    }

                    db.SaveChanges();
                }
                else if (pro != "" && pro != null)
                {
                    string[] ps = pro.Split(',');
                    int storageId = int.Parse(ps[0]);
                    int countNum = int.Parse(ps[1]);

                    var proSorage = db.ProductStorages.Find(storageId);
                    var proInfo = proSorage.ProductInfo;
                    var proColor = proSorage.ProductColor;
                    if (countNum > proSorage.StorageNumber)
                    {
                        return Content("NotStorage");
                    }
                    proSorage.StorageNumber = proSorage.StorageNumber - countNum;

                    order = new Order();

                    //赵志康 158****2144   辽宁  本溪市  平山区  阿萨德阿萨德按时
                    var address = db.Addresses.SingleOrDefault(a => a.AddressId == addressId);
                    order.AddressDetails = address.AddressName + " " + address.AddressPhone + " " +
                        address.AddressProvince + " " + address.AddressCity + " " + address.AddressCounty + " " + address.AddressDetail;

                    long time = PublicController.TimeStamp();
                    orderIds += time + "；";
                    order.OrderNumber = Convert.ToString(time);

                    order.MemberId = memberId;
                    order.OrderCount = countNum;
                    order.ColorName = proColor.ColorName;
                    order.OrderPrice = proInfo.ProPrice;
                    order.OrderProCoverImage = proInfo.ProCoverImage;
                    order.OrderProIntroduce = proInfo.ProFeature;
                    order.OrderProName = proInfo.ProName;
                    order.OrderStatusId = 1;
                    order.OrderTime = DateTime.Now.ToString();
                    order.ProId = proInfo.ProId;
                    order.StorageId = storageId;

                    db.Orders.Add(order);
                    db.Entry(order).State = EntityState.Added;
                 
                    db.SaveChanges();
                }
            }
            catch (Exception) {
                return Content("SystemError");
            }
            
            return Content(orderIds);
        }

        public string CheckCartNum(string cartInfo="") { 
            string[] carts = cartInfo.Split(',');
            for (int i = 0; i < carts.Length - 1; i++)
            {           
                int cartId = int.Parse(carts[i]);
                var x = db.Carts.Find(cartId);
                if (x == null){
                    return "DoubleSubmit";
                }
                
                var proSorage = db.ProductStorages.Find(x.StorageId);
                if (x.CartCount > proSorage.StorageNumber)
                {
                    return "NotStorage";
                }                
            }
            return "True";
        }

        public void CheckCart(int memberId) { 
           
            try
            {

                var carts = db.Carts.Where(i => i.MemberId == memberId );
                foreach (var x in carts) {
                    if (DateTime.Now > x.CartTime.AddHours(2)) {
                        db.Carts.Remove(x);
                        db.Entry(x).State = EntityState.Deleted;
                    }                  
                }
                db.SaveChanges();
            }
            catch (Exception) { 
                
            }
        }


        public ActionResult SubmitPayOrder(string orderIds = "",decimal priceCount=0, string payUsername = "", string payPwd = "")
        {
            //修改订单状态，减少支付通金额，记录支付记录。          
            if (checkOrderNoPay(orderIds))
            {
                try
                {
                    var payment = db.Payments.SingleOrDefault(i => i.PaymentUserName == payUsername);
                    if (payment.PaymentPayPssword != payPwd)
                    {
                        return Content("PwdError");
                    }
                    if (payment.PaymentBalance < priceCount) {
                        return Content("NoPayPrice");
                    }

                    PaymentLog payLog = null;
                    string[] orders = orderIds.Split('；');
                    for (int i = 0; i < orders.Length - 1; i++)
                    {
                        string orderNumber = orders[i];
                        var x = db.Orders.SingleOrDefault(s => s.OrderNumber == orderNumber);                
                        x.OrderStatusId=2;
                        x.OrderPayTime = DateTime.Now.ToString();

                        payLog = new PaymentLog();
                        payLog.PaymentId = payment.PaymentId;
                        payLog.PaymentLogName = "购买商品："+x.OrderProName+" "+x.ColorName;
                        payLog.PaymentLogPrice = -(x.OrderPrice * x.OrderCount);
                        payLog.PaymentLogTime = DateTime.Now;

                        db.PaymentLog.Add(payLog);
                        db.Entry(payLog).State = EntityState.Added;
                    }
                    payment.PaymentBalance = payment.PaymentBalance - priceCount;
                    db.Entry(payment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return Content("SystemError");
                }
            }
            else {
                return Content("AlreadyPay");
            }

            return Content("True");
        }

        public bool checkOrderNoPay(string orderIds = "")
        {
            string[] orders = orderIds.Split('；');
            for (int i = 0; i < orders.Length - 1; i++)
            {
                string orderNumber = orders[i];
                var x = db.Orders.SingleOrDefault(s => s.OrderNumber == orderNumber);
                if (x == null)
                {
                    return false;
                }
                else if(x.OrderStatusId != 1){
                    return false;
                }
            }
            return true;
        }


        public ActionResult CancelOrder(int orderId=-1) {            
            //判断orderid是1还是2，，
            //如果是1,则设置状态为5,把数量返还给库存
            //如果是2，则设置状态为5,把数量返还给库存，并把钱返还给账户
            try
            {
                var order = db.Orders.Find(orderId);

                if (order.OrderStatusId == 1) {
                    var storage = db.ProductStorages.Find(order.StorageId);
                    storage.StorageNumber=storage.StorageNumber + order.OrderCount;
                    order.OrderStatusId = 5;
                }
                else if (order.OrderStatusId == 2)
                {
                    var storage = db.ProductStorages.Find(order.StorageId);
                    storage.StorageNumber = storage.StorageNumber + order.OrderCount;

                    var payMent = db.Payments.SingleOrDefault(p => p.MemberId == order.MemberId);
                    payMent.PaymentBalance = payMent.PaymentBalance + (order.OrderPrice * order.OrderCount);

                    PaymentLog payLog = new PaymentLog();
                    payLog.PaymentId = payMent.PaymentId;
                    payLog.PaymentLogName = "取消订单："+order.OrderProName+" "+order.ColorName;
                    payLog.PaymentLogPrice = +(order.OrderPrice * order.OrderCount);
                    payLog.PaymentLogTime = DateTime.Now;

                    db.PaymentLog.Add(payLog);
                    db.Entry(payLog).State = EntityState.Added;
                    order.OrderStatusId = 5;
                }
                else {
                    return Content("订单状态已经改变，请刷新页面！");
                }

                db.SaveChanges();
            }
            catch (Exception) {
                return Content("系统出错，怪我咯！");
            }
            return Content("True");
        }

        public ActionResult ReceivingGoods(int orderId)
        {
            //1.修改状态和插入成交时间
            try
            {
                var order = db.Orders.Find(orderId);
                if (order.OrderStatusId == 3)
                {
                    order.OrderStatusId = 4;
                    order.OrderSuccessTime = DateTime.Now.ToString();
                    db.Entry(order).State = EntityState.Modified;

                    var pro=db.ProductInfos.Find(order.ProId);
                    pro.ProSales = pro.ProSales + order.OrderCount;
                    db.Entry(pro).State = EntityState.Modified;
                }
                else
                {
                    return Content("订单状态已经改变，请刷新页面！");
                }
                db.SaveChanges();
            }
            catch (Exception) {
                return Content("系统出错，怪我咯！");
            }
            return Content("True");       
        }

        public ActionResult ReturnGoods(int orderId)
        {
            //1.修改状态即可。        
            //2.收货后，7天内才可退货
            try
            {
                var order = db.Orders.Find(orderId);
                if (order.OrderStatusId == 4)
                {
                    if (DateTime.Now > DateTime.Parse(order.OrderSuccessTime).AddDays(7)) {
                        return Content("收货已超过7天，不允许退货了！");
                    }
                    order.OrderStatusId = 6;
                    db.Entry(order).State = EntityState.Modified;
                }
                else
                {
                    return Content("订单状态已经改变，请刷新页面！");
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("系统出错，怪我咯！");
            }
            return Content("True");

        }
     
        public ActionResult SubmitRecharge(int payMentId=0,decimal payPrice=0,string payPwd="") {
            try
            {
                if (payPrice < 100 || payPrice > 10000)
                    return Content("充值金额有误，请重新填写！");

                var payment = db.Payments.Find(payMentId);
                if (payment.PaymentPayPssword != payPwd) 
                    return Content("交易密码有误，请重新填写！");
                
                payment.PaymentBalance=payment.PaymentBalance+payPrice;
                db.Entry(payment).State = EntityState.Modified;

                PaymentLog pl = new PaymentLog();
                pl.PaymentId = payMentId;
                pl.PaymentLogName = "充值金额";
                pl.PaymentLogPrice = payPrice;
                pl.PaymentLogTime = DateTime.Now;
                db.Entry(pl).State = EntityState.Added;

                db.SaveChanges();
            }
            catch (Exception) {
                return Content("系统出错，怪我咯！");
            }

            return Content("True");
        }
          
        public ActionResult OrderEditAddress(int OrderId=0,string addressName="暂无",string addressPhone="暂无",string addressDetail="暂无") {

            try
            {
                var order = db.Orders.Find(OrderId);
                string address = addressName + " " + addressPhone + " " + addressDetail;
                order.AddressDetails = address;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("系统出错，怪我咯！");
            }
            return Content("True");
        }

        public ActionResult EditPayPwd(int payMentId = 0, string oldPwd="",string newPwd = "")
        {           
            try
            {                   
                var payment = db.Payments.Find(payMentId);
                if (payment.PaymentPayPssword != oldPwd)
                    return Content("原始交易密码不正确，请重新填写！");

                payment.PaymentPayPssword = newPwd;

                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Content("系统出错，怪我咯！");
            }

            return Content("True");
        }


        public ActionResult MemberUploadPhoto()
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

                    Bitmap s = PublicController.SizeImage(myImg, 300, 300);

                    s.Save(Server.MapPath("../Images/web/memberImg/" + filename + ".png"));

                    //PublicController.SaveFormatJpeg(s, Server.MapPath("../Images/web/memberImg/" + filename + ".jpeg"), 100L);
                    s.Dispose();

                    db.Members.Find(int.Parse(Session["MemberId"].ToString())).MemberPhoto = filename + ".png";
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpException();
                }
            }
            catch (FileNotFoundException e)
            {            
                return Content("False");
            }
            catch (HttpException e)
            {                               
                return Content("PicTooBig");
            }
            finally
            {             
                if (myImg != null)
                    myImg.Dispose();
            }          
            return Content("/Images/web/upload/" + filename + ".png");
        }
    }
}
