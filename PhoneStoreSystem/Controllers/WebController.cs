using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneStoreSystem.Models;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Web.Routing;
using PhoneStoreSystem.Filters;
using NewsSystem.Controllers;

namespace PhoneStoreSystem.Controllers
{

    public class WebController : Controller
    {
        //
        // GET: /Web/
        PhoneStoreContext db = new PhoneStoreContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ProductList(int pageSize = 10, int pageIndex = 1, int proTypeId = -1, int proPriceId=-1 ,int proOrderByType=0, int proOrderBy =-1)
        {
       
            var val = (from s in db.ProductInfos
                       join t in db.ProductTypes 
                       on s.TypeId equals (proTypeId == -1 ? t.TypeId : proTypeId)   
                       where s.ProIsSale==true
                       select s).Distinct();          
                 
            if (proPriceId != -1) {
                val = val.Where(s => s.ProPrice >= proPriceId && s.ProPrice < (proPriceId == 5000 ? int.MaxValue : (proPriceId + 1000)));
            }
            
            ViewBag.listCount = val.Distinct().Count();
            ViewBag.pageCount = (ViewBag.listCount - 1) / pageSize + 1;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            ViewBag.proTypeId = proTypeId;          
            ViewBag.proPriceId = proPriceId;
            ViewBag.proOrderByType = proOrderByType;
            ViewBag.proOrderBy = proOrderBy; //1 2 3 4

            /*
            ArrayList al=new ArrayList();
            IList li;        
            foreach (var x in db.ProductTypes) {
                li = new ArrayList();
                li.Add(x.TypeName);
                li.Add(x.TypeId);
                al.Add(li);
            }
            ViewBag.proTypeList = al;
            */

            ViewBag.proTypeList = db.ProductTypes;



            if (proOrderByType == 0)
            {

                switch (proOrderBy)
                {
                    case 1: return View(val.OrderByDescending(i => i.ProReleaseDate).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));    //1发布时间排序
                    case 2: return View(val.OrderByDescending(i => i.ProSaleDate).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));    //2上架时间排序
                    case 3: return View(val.OrderByDescending(i => i.ProSales).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //3销售量排序
                    case 4: return View(val.OrderByDescending(i => i.ProReadNumber).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //4浏览数排序
                    case 5: return View(val.OrderByDescending(i => i.ProPrice).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //5价格排序
                }

                return View(val.OrderByDescending(i => i.ProId).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));
            }
            else {

                switch (proOrderBy)
                {
                    case 1: return View(val.OrderBy(i => i.ProReleaseDate).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));    //1发布时间排序
                    case 2: return View(val.OrderBy(i => i.ProSaleDate).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));    //2上架时间排序
                    case 3: return View(val.OrderBy(i => i.ProSales).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //3销售量排序
                    case 4: return View(val.OrderBy(i => i.ProReadNumber).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //4浏览数排序
                    case 5: return View(val.OrderBy(i => i.ProPrice).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize)); //5价格排序
                }

                return View(val.OrderBy(i => i.ProId).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));

            }          
           
        }

        [AllowAnonymous]   
        public ActionResult ProductDetail(int id=-1)
        {
            ProductInfo proInfoDetail = db.ProductInfos.Find(id);
            
            if (proInfoDetail == null)
            {
                throw new HttpException(404, "page not found");
            }

            proInfoDetail.ProReadNumber = proInfoDetail.ProReadNumber + 1;
            db.Entry(proInfoDetail).State = EntityState.Modified;
            db.SaveChanges();
            var productStorage = db.ProductStorages.Include(p => p.ProductColor).Where(i => i.ProId == id);
            ViewBag.productStorage = productStorage;

            ViewBag.productHot = db.ProductInfos.Where(p=>p.ProIsSale==true).OrderByDescending(p=>p.ProReadNumber).Take(5);
            return View(proInfoDetail);
            
        }

        
        public ActionResult MemberCenter() {
            int memberId = int.Parse(Session["MemberId"].ToString());
           
            var member = db.Members.Find(memberId);
            var orders = db.Orders.Where(o => o.MemberId == memberId).Select(o=>o.OrderStatusId);

            int noPayNum=0, noReceivingNum=0, successNum=0, returnNum=0;
            foreach (var i in orders) {
                switch (i) {
                    case 1: noPayNum += 1; break;
                    case 3: noReceivingNum += 1; break;
                    case 4: successNum += 1; break;
                    case 7: returnNum += 1; break;                      
                }
            }
            ViewBag.NoPayNum = noPayNum;
            ViewBag.NoReceivingNum = noReceivingNum;
            ViewBag.SuccessNum = successNum;
            ViewBag.ReturnNum = returnNum;
            return View(member);
        }


        public ActionResult MemberOrder(int pageSize = 3, int pageIndex = 1, int orderStatusId = -1, string keywords="")
        {
            int memberId = int.Parse(Session["MemberId"].ToString());
            

            var orders = db.Orders.Where(o => o.MemberId == memberId);
            if (orderStatusId != -1)
                orders=orders.Where(o => o.OrderStatusId == orderStatusId);

            if (!string.IsNullOrEmpty(keywords)) {
                orders = orders.Where(o => o.OrderNumber.Contains(keywords.Trim()) || o.OrderProName.Contains(keywords.Trim()));
            }


            ViewBag.listCount = orders.Distinct().Count();
            ViewBag.pageCount = (ViewBag.listCount - 1) / pageSize + 1;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            ViewBag.OrderStatusId = orderStatusId; //1 2 3 4 5 6 7
            ViewBag.Keywords = keywords;

            
            return View(orders.OrderByDescending(i => i.OrderId).ToList().Skip(pageSize * (pageIndex - 1)).Take(pageSize));
        }

      
        public ActionResult MemberOrderDetail(int id) {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                throw new HttpException(404, "page not found");
            }
            return View(order);
        }

      
        public ActionResult MemberCart() {

            int memberId = int.Parse(Session["MemberId"].ToString());

            new WebPublicController().CheckCart(memberId);

            
          
            var carts = db.Carts.Where(c => c.MemberId == memberId).Include(c => c.ProductInfo).Include(c=>c.ProductColor);

            ViewBag.CartCount = carts.Count();            
            return View(carts);       
        }
        
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberCheckOrder()
        {
            int memberId = int.Parse(Session["MemberId"].ToString());
            
            int countNumber = 0;

            IList<CheckOrder> list = new List<CheckOrder>();
            CheckOrder co=null;
            decimal countPrice = 0;      
     
            string cartInfo = Request.Form["CartsInfo"];
            string proInfo = Request.Form["ProInfo"];

            if (cartInfo != null)
            {
                string[] carts = cartInfo.Split(',');
                for (int i = 0; i < carts.Length - 1; i++)
                {
                    int cartId = int.Parse(carts[i]);
                    var x = db.Carts.Find(cartId);
                    if (x == null)
                    {
                        return RedirectToAction("MemberCart", "Web");
                    }

                    co = new CheckOrder();
                    co.ProId = x.ProId;
                    co.StorageId = x.StorageId;
                    co.ColorId = x.ColorId;
                    co.ProName = x.ProductInfo.ProName + " - " + x.ProductColor.ColorName;
                    co.ProPrice = x.ProductInfo.ProPrice;
                    co.ProNum = x.CartCount;
                    co.StorageNum = db.ProductStorages.Find(co.StorageId).StorageNumber;


                    countPrice += co.ProPrice * co.ProNum;
                    countNumber += co.ProNum;
                    list.Add(co);
                }                
            }
            else if (proInfo!=null) //点击立即购买执行
            {
                string[] pros = proInfo.Split(',');
                int storageId = int.Parse(pros[0]);         
                int countNum = int.Parse(pros[1]);

                var x = db.ProductStorages.Find(storageId);
                co = new CheckOrder();
                co.ProId = x.ProId;
                co.StorageId = x.StorageId;
                co.ColorId = x.ColorId;
                co.ProName = x.ProductInfo.ProName + " - " + x.ProductColor.ColorName;
                co.ProPrice = x.ProductInfo.ProPrice;
                co.ProNum = countNum;
                co.StorageNum = db.ProductStorages.Find(co.StorageId).StorageNumber;

                countPrice += co.ProPrice * co.ProNum;
                countNumber += co.ProNum;
                list.Add(co);              
            }
           
            ViewBag.Address=db.Addresses.Where(i => i.MemberId == memberId);
            ViewBag.CountPrice = countPrice;
            ViewBag.CountNumber = countNumber;
            ViewBag.CartInfo = cartInfo;
            ViewBag.ProInfo = proInfo;
            return View(list);
        }

        public ActionResult MemberPayOrder() {

            Uri u = HttpContext.Request.UrlReferrer;
            if ( u != null)
            {                 
                if ((u.LocalPath.Contains("/Web/MemberCheckOrder") || u.LocalPath.Contains("/Web/MemberOrder")) || u.LocalPath.Contains("/Web/MemberOrderDetail"))
                {
                    int memberId = int.Parse(Session["MemberId"].ToString());
               
                    int paymentId = db.Members.Find(memberId).PaymentId;
                    if (paymentId == 0) {
                        return RedirectToAction("MemberPay", "Web");
                    }
                    string paymentUsername = db.Payments.Find(paymentId).PaymentUserName;

                    string orderIds = Request.Form["OrderIds"];
                    string[] orderId = orderIds.Split('；');
                    string addressDetail="";
                    string proNames = "";
                    decimal proPriceCount = 0;

                    for (int i = 0; i < orderId.Length - 1; i++) {
                        string id = orderId[i];
                        var order = db.Orders.SingleOrDefault(o => o.OrderNumber == id);
                        if (i == 0)
                        {
                            addressDetail = order.AddressDetails;
                        }
                        proNames += order.OrderProName + " " + order.ColorName + "；";
                        proPriceCount += order.OrderCount * order.OrderPrice;
                    }
                    
                    ViewBag.OrderIds = orderIds;
                    ViewBag.AddressDetail = addressDetail;
                    ViewBag.ProNames = proNames;
                    ViewBag.ProPriceCount = proPriceCount;
                    ViewBag.PaymentUsername = paymentUsername;
                    
                }
                else
                {
                    return RedirectToAction("Index", "Web");
                }
            }
            else
            {
                return RedirectToAction("Index", "Web");
            }
            
            return View();
        }

        public ActionResult MemberAddress()
        {
            int memberId = int.Parse(Session["MemberId"].ToString());
            var address=db.Addresses.Where(a=>a.MemberId==memberId);

            return View(address);
        }

        public ActionResult MemberAddAddress()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MemberAddAddress(Address address )
        {
            if (ModelState.IsValid)
            {
                try{
                    int memberId = int.Parse(Session["MemberId"].ToString());

                    if (db.Addresses.Where(a => a.MemberId == memberId).Count() >= 10)
                    {
                        ViewBag.StatusMessage = "您的地址数量已经达到10条，不能再新增了！";
                        return View(address);
                    }
                    db.Addresses.Add(address);

                    db.Entry(address).State = EntityState.Added;
                    db.SaveChanges();

                    return RedirectToAction("/MemberAddress");
                }
                catch (Exception)
                {
                    ViewBag.StatusMessage = "系统错误，请刷新重试！";
                }              
            }
            else {
                ViewBag.StatusMessage = "验证失败，请检查你的数据！";
            }

            return View(address);
        }
        public ActionResult MemberEditAddress(int id) {
            var address = db.Addresses.Find(id);
            if (address == null)
            {
                //HttpContext.Response.StatusCode=404;
                throw new HttpException(404, "page not found");//404页面
            }
            return View(address);
        }

        [HttpPost]
        public ActionResult MemberEditAddress(Address address)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(address).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("/MemberAddress");
                }
                catch (Exception)
                {
                    ViewBag.StatusMessage = "系统错误，请刷新重试！";
                }

            }
            else
            {
                ViewBag.StatusMessage = "验证失败，请检查你的数据！";
            }
        
            return View(address);
        }

     
        public ActionResult MemberRemoveAddress(int addressId) {
            try
            {
                var address = db.Addresses.Find(addressId);
                db.Addresses.Remove(address);
                db.Entry(address).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception){
                return Content("移除失败，请刷新再重试！");
            }
            return Content("True");
        }

     
        public ActionResult MemberPay()
        {
            int memberId = int.Parse(Session["MemberId"].ToString());
            var payMent = db.Payments.SingleOrDefault(p=>p.MemberId==memberId);

            if (payMent != null)
            {
                ViewBag.listCount = db.PaymentLog.Where(p => p.PaymentId == payMent.PaymentId).ToList().Count();
                ViewBag.pageIndex = 0;
                ViewBag.IsHavePayment = 1;
                ViewBag.PayMentId = payMent.PaymentId;
            }else{
                ViewBag.IsHavePayment = 0;
                ViewBag.PayMentId = 0;
            }
            
            return View(payMent);
        }

      
        public ActionResult GetPageListToPaylog(int pageIndex = 0, int pageSize = 10, int payMentId=0)
        {
            //默认采用创建代理的方式返回实体集合，这里必须要关闭才可以读出数据
            db.Configuration.ProxyCreationEnabled = false;

            var payLogs = from n in db.PaymentLog where n.PaymentId==payMentId select n;


            return Json(payLogs.OrderByDescending(i => i.PaymentLogTime).ToList().Skip(pageIndex * pageSize).Take(pageSize), JsonRequestBehavior.AllowGet);

        }

        public ActionResult MemberEditPay(int id)
        {
            var payment = db.Payments.Find(id);
            if (payment == null)
            {
                //HttpContext.Response.StatusCode=404;
                throw new HttpException(404, "page not found");//404页面
            }
            return View(payment);
        }

        [HttpPost]
        public ActionResult MemberEditPay(Payment Payment)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(Payment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("/MemberPay");
                }
                catch (Exception)
                {
                    ViewBag.StatusMessage = "系统错误，请刷新重试！";
                }

            }
            else
            {
                ViewBag.StatusMessage = "验证失败，请检查你的数据！";
            }

            return View(Payment);
        }

        public ActionResult MemberPayRegister()
        {
            return View();
        }     

        [HttpPost]
        public ActionResult MemberPayRegister(Payment payment)
        {            
            try
            {
                int memberId = int.Parse(Session["MemberId"].ToString());
                payment.MemberId = memberId;
                payment.PaymentBalance = 0;
                payment.PaymentPayPssword = "123456";

                db.Entry(payment).State = EntityState.Added;
                db.SaveChanges();


                db.Members.Find(memberId).PaymentId = payment.PaymentId;
                db.SaveChanges();
               

                return RedirectToAction("/MemberPay");
            }
            catch (Exception)
            {
                ViewBag.StatusMessage = "系统错误，请刷新重试！";
            }

            return View(payment);
        }

        //用于检测是否存在
        public JsonResult IsExistPayment(string paymentUserName)
        {
            bool isExist = true;
            try
            {
                string[] url = Request.UrlReferrer.LocalPath.Split('/');
                string actionName = url[2];
                if (actionName == "MemberPayRegister")
                {
                    var u = from n in db.Payments
                            where n.PaymentUserName == paymentUserName
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
                else if (actionName == "MemberEditPay")
                {
                    int id = int.Parse(url[url.Length - 1]);

                    var u = from n in db.Payments
                            where n.PaymentUserName == paymentUserName && n.PaymentId != id
                            select n; ;
                    if (u.Count() != 0)
                    {
                        isExist = false;
                    }
                }
            }
            catch
            {
                var u = from n in db.Payments
                        where n.PaymentUserName == paymentUserName
                        select n; ;
                if (u.Count() != 0)
                {
                    isExist = false;
                }
            }

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult MemberInfo()
        {
            int memberId = int.Parse(Session["MemberId"].ToString());
            var member = db.Members.Find(memberId);
            return View(member);
        }      
        
        [HttpGet]
        public ActionResult MemberEditInfo(int id)
        {
            var member = db.Members.Find(id);
            if (member == null)
            {
                //HttpContext.Response.StatusCode=404;
                throw new HttpException(404, "page not found");//404页面
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("MemberEditInfo")]
        public ActionResult MemberEditInfoSB(Member member)
        {                     
            try
            {
                var m = db.Members.Find(member.MemberId);
                m.MemberPhone = member.MemberPhone;
                m.MemberRealName = member.MemberRealName;
                m.MemberSex = member.MemberSex;
                m.MemberEmail = member.MemberEmail;
                m.MemberBirthDate = member.MemberBirthDate;

                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("/MemberInfo");
            }
            catch (Exception e)
            {
                ViewBag.StatusMessage = "系统错误，请刷新重试！";
            }
            

            return View(member);
        }
        

        public ActionResult MemberEditPwd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberEditPwd(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // 在某些失败方案中，ChangePassword 将引发异常，而不是返回 false。
                bool changePasswordSucceeded;
                try
                {
                    int memberId = int.Parse(Session["MemberId"].ToString());
                    var member = db.Members.Find(memberId);
                    if (member.MemberPassword == model.OldPassword)
                    {
                        member.MemberPassword = model.NewPassword;

                        db.Entry(member).State = EntityState.Modified;
                        db.SaveChanges();

                        changePasswordSucceeded = true;
                    }
                    else
                    {
                        changePasswordSucceeded = false;
                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    ViewBag.StatusMessage = "修改密码成功！";                  
                }
                else
                {
                    ModelState.AddModelError("", "当前密码不正确或新密码无效。");
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult MemberLogin()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult MemberLogin(MemberLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var m = from n in db.Members
                        where n.MemberName == model.MemberName && n.MemberPassword == model.MemberPassword
                        select n;
                if (m.Count() != 0)
                {
                    Member member =m.First();

                    Session.Add("MemberId", member.MemberId);
                    Session.Add("MemberName", member.MemberName);
                    //Session.Timeout = 3600;
                    return RedirectToLocal(returnUrl);
                }

            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View(model);

        }
       
        [AllowAnonymous]
        public ActionResult MemberRegister()
        {           
            return View();
        }
        [HttpPost]     
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult MemberRegister(MemberRegisterModel model)
        {       
            if (ModelState.IsValid)
            {
                // 尝试注册用户
                try
                {
                    Member member = new Member();
                    member.MemberName = model.MemberName;
                    member.MemberPassword = model.Password;
                    member.MemberRealName = model.RealName;
                    member.MemberPhone = model.MemberPhone;
                    member.MemberEmail = model.MemberEmail;
                    member.MemberBirthDate = DateTime.Now;
                    member.MemberPhoto = "../photo.png";
                    member.MemberSex = true;
                    member.PaymentId = 0;
                    member.MemberRegisterTime = DateTime.Now;
                    
                    db.Entry(member).State=EntityState.Added;
                    db.SaveChanges();

                    return RedirectToAction("MemberLogin", "Web");
                    
                }
                catch (Exception)
                {                   
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "表单信息错误，请检查！");
            return View(model);
        }

        //用于检测是否存在
        [AllowAnonymous]
        public JsonResult IsExistMember(string memberName)
        {
            bool isExist = true;
          
            var u = from n in db.Members
                    where n.MemberName == memberName
                    select n; ;
            if (u.Count() != 0)
            {
                isExist = false;
            }
                
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            
            Session["MemberId"] = null;
            Session["MemberName"] = null;         
                     
            return RedirectToAction("MemberLogin", "Web");
        }


        private ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Web");
            }
        }


        [AllowAnonymous]
        public ActionResult ProductSearch(/*int pageSize = 20, int pageIndex = 1, */string keywords = "", int CurrentRank = 9999)
        {
            
            keywords = keywords.Trim();
            if (!string.IsNullOrEmpty(keywords))
            {          
                string sqlCount = null;
                string sql = null;
                //string sqlRank = null;
                //string where = "ProName,ProFeature";
                /*
                //获取总数
                sqlCount = @"SELECT  COUNT(ProId)  FROM  db_ProductInfo AS products
                            INNER JOIN FREETEXTTABLE(	
		                        db_ProductInfo, 
		                        (" + where + @"),
		                        '" + keywords + @"'
		                        )  
                            AS KEY_TBL		
                            ON products.ProId = KEY_TBL.[KEY]";

                //获取当前用于分页的最高的rank值
                sqlRank = @"SELECT  top " + (pageIndex * pageSize) + @" KEY_TBL.rank  FROM db_ProductInfo AS products
                            INNER JOIN FREETEXTTABLE(	
		                        db_ProductInfo, 
		                        (" + where + @"),
		                        '" + keywords + @"'	
		                        )  
                            AS KEY_TBL		
                            ON products.ProId = KEY_TBL.[KEY]
                        ORDER BY KEY_TBL.RANK DESC	";


                DataTable dtRank = SqlHelperController.GetDataTable(sqlRank, null);
                if (dtRank.Rows.Count > 0)
                    CurrentRank = int.Parse(dtRank.Rows[pageSize * (pageIndex - 1)]["rank"].ToString());

                //执行筛选
                sql = @"SELECT top " + pageSize + @" ProId,ProName,ProFeature,ProCoverImage,ProReleaseDate,ProSaleDate,ProPrice,rank FROM db_ProductInfo AS products
                        INNER JOIN FREETEXTTABLE(	
		                    db_ProductInfo, 
		                    (" + where + @"),
		                    '" + keywords + @"'
		                    )  
                        AS KEY_TBL		
                        ON products.ProId = KEY_TBL.[KEY] 
                        where  KEY_TBL.RANK <=" + CurrentRank + @"
                    ORDER BY KEY_TBL.RANK DESC";
                */
                //ViewBag.listCount = SqlHelperController.GetDataTable(sqlCount, null).Rows[0][0];

                sql = @"SELECT ProId,ProName,ProFeature,ProCoverImage,ProReleaseDate,ProSaleDate,ProPrice,rank FROM db_ProductInfo AS products
                        INNER JOIN FREETEXTTABLE(	
                            db_ProductInfo, 
                            (ProName,ProFeature),
                            '" + keywords + @"',20
                            )  
                        AS KEY_TBL		
                        ON products.ProId = KEY_TBL.[KEY] 
                        ORDER BY KEY_TBL.RANK DESC";
               

                DataTable dt = SqlHelperController.GetDataTable(sql, null);
                if (dt.Rows.Count == 0) {
                    sql = @"select top 20 ProId,ProName,ProFeature,ProCoverImage,ProReleaseDate,ProSaleDate,ProPrice 
                            FROM db_ProductInfo AS products
                            where ProName like '%" + keywords + @"%' or ProFeature like '%" + keywords + @"%' 
                            order by proReadNumber desc";
                    dt=SqlHelperController.GetDataTable(sql, null);
                }
                ViewData["searchData"] = dt;
                ViewBag.listCount = dt.Rows.Count;
            }
            else
            {
                ViewBag.listCount = 0;              
            }

            //ViewBag.pageCount = (ViewBag.listCount - 1) / pageSize + 1;
            //ViewBag.pageIndex = pageIndex;
            ViewBag.keywords = keywords;
            

            return View();

        }
        
    }
}
