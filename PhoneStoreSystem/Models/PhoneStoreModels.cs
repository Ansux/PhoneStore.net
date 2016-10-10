using PhoneStoreSystem.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PhoneStoreSystem.Models
{
    public class PhoneStoreContext : DbContext
    {
        public PhoneStoreContext()
            : base("PhoneStoreDBConnection")
        {
        }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<RoleInfo> RoleInfos { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductInfo> ProductInfos { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<ProductStorage> ProductStorages { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentLog> PaymentLog { get; set; }

        
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Navigation> Navigations { get; set; }

        public DbSet<Slide> Slides { get; set; }

    }


    [Table("db_ProductType")]
    public class ProductType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int TypeId { get; set; }

        [Required]
        [StringLength(10)]    
        [Display(Name = "分类名称")]
        [System.Web.Mvc.Remote("IsExistProType", "ProductType", ErrorMessage = "该类型已存在！")]  
        public string TypeName { get; set; }

        public virtual ICollection<ProductInfo> ProductInfos { get; set; }

    }

    [Table("db_ProductInfo")]
    public class ProductInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProId { get; set; }

        
        [Display(Name="是否上架")]       
        [ScaffoldColumn(false)]
        public bool ProIsSale { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "产品型号")]
        [System.Web.Mvc.Remote("IsExistProName", "ProductInfo", ErrorMessage = "该名称已存在！")]  
        public string ProName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "产品特色")]
        public string ProFeature { get; set; }

      
        [StringLength(100)]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "产品封面")]
        public string ProCoverImage { get; set; } 

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "产品介绍")]
        public string ProIntroduce { get; set; }

  
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "产品发布日期")]
        public DateTime ProReleaseDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "产品单价")]
        public decimal ProPrice { get; set; }

        [Required]
        [Display(Name = "销售量")]
        [ScaffoldColumn(false)]
        public int ProSales{ get; set; }

        [DataType(DataType.DateTime)]
        [ScaffoldColumn(false)]
        [Display(Name = "商城上架日期")]
        public DateTime ProSaleDate { get; set; }

        [Required]
        [StringLength(10)]
        [ScaffoldColumn(false)]
        [Display(Name = "发布人")]
        public string ProAuthor { get; set; }

        [Required]
        [Display(Name = "浏览次数")]
        [ScaffoldColumn(false)]     //设置为false后，就不会在页面中自动生成这个字段了。。
        public int ProReadNumber { get; set; }

        [Required]
        [Display(Name = "产品类型")]
        public int TypeId { get; set; }

       
        public virtual ProductType ProductType { get; set; }    

    }


    [Table("db_ProductColor")]
    public class ProductColor
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int ColorId { get; set; }

        [Required]
        [StringLength(10)] 
        [Display(Name = "颜色名称")]
        [System.Web.Mvc.Remote("IsExistProColor", "ProductColor", ErrorMessage = "该颜色已存在！")]  
        public string ColorName { get; set; }
    }

    [Table("db_ProductStorage")]
    public class ProductStorage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int StorageId { get; set; }

        [Required]
        [Display(Name = "库存量")]
        public int StorageNumber { get; set; }
  
        [StringLength(100)]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "产品图片")]
        public string ProImage { get; set; }

        [Required]
        [Display(Name = "产品类型")]
        public int ProId { get; set; }

        [Required]
        [Display(Name = "产品颜色")]
        public int ColorId { get; set; }

        public virtual ProductInfo ProductInfo { get; set; }

        public virtual ProductColor ProductColor { get; set; } 
    }

    public class ProductStorageAndStyle
    {
        public int id { get; set; }
        public string ProImage { get; set; }
        public int StorageNumber { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
    }

    [Table("db_Member")]
    public class Member {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int MemberId { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        [Display(Name = "用户名")]       
        public string  MemberName { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "{0} 必须为{2}-{1} 个字符。", MinimumLength = 6)]
        [Display(Name="密码")]
        public string MemberPassword { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "真实姓名")]
        public string MemberRealName { get; set; }

        [Required]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "格式不正确")]
        [Display(Name = "手机号码")]
        public string MemberPhone { get; set; }

     
        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确的邮箱地址")]
        [Display(Name = "邮箱")]
        public string MemberEmail { get; set; }

        [Required]
        [Display(Name = "出生日期")]
        [DataType(DataType.DateTime)]
        public DateTime MemberBirthDate { get; set; }

       
        [Display(Name = "性别")]
        public bool MemberSex { get; set; }

  
        [Display(Name="相片")]
        [StringLength(100)]
        public string MemberPhoto { get; set; }

        [DataType(DataType.DateTime)]
        [ScaffoldColumn(false)]
        [Display(Name = "注册日期")]
        public DateTime MemberRegisterTime { get; set; }

        
        [Required]
        [Display(Name = "支付通")]
        public int PaymentId { get; set; }       //初始为0

    }

    public class MemberLoginModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string MemberName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string MemberPassword { get; set; }

    }

    public class MemberRegisterModel
    {
        [Required]
        //[LoginNameUnique]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "请输入{2}-{1} 个字符。")]
        [Display(Name = "用户名")]
        [System.Web.Mvc.Remote("IsExistMember","Web",ErrorMessage = "该会员名已存在！")]  
        public string MemberName { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "请输入{2}-{1} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "真实姓名")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "请输入{2}-{1} 个字符。")]
        public string RealName { get; set; }


        [Required]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "格式不正确")]
        [Display(Name = "手机号码")]
        public string MemberPhone { get; set; }

        [Required]
        [StringLength(50,ErrorMessage = "最多允许50个字符")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确的邮箱地址")]
        [Display(Name = "邮箱")]
        public string MemberEmail { get; set; }
    }


    [Table("db_Address")]
    public class Address
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int AddressId { get; set; }

        [Required(ErrorMessage="*")]
        [Display(Name = "详细地址")]
        [StringLength(32,MinimumLength=5,ErrorMessage = "请输入5-32个字符")]
        public string AddressDetail { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "省份")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "请输入2-10个字符")]
        public string AddressProvince { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "城市")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "请输入2-10个字符")]
        public string AddressCity { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "区/县")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "请输入2-10个字符")]
        public string AddressCounty { get; set; }


        [Required(ErrorMessage = "*")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "邮编")]
        public string AddressCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "联系人")]
        [StringLength(10)]
        public string AddressName { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "格式不正确")]
        [Display(Name = "联系电话")]
        public string AddressPhone { get; set; }

        [Required]
        [Display(Name="会员名")]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; } 

    }

    [Table("db_Payment")]
    public class Payment {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int PaymentId { get; set; }

        [Required]
        [Display(Name = "帐号")]
        [StringLength(16,ErrorMessage="*不能超过16个字符")]
        [System.Web.Mvc.Remote("IsExistPayment", "Web", ErrorMessage = "该帐号已存在！")]  
        public string PaymentUserName { get; set; }

        [Required]
        [StringLength(10,ErrorMessage="*不能超过10个字符")]
        [Display(Name = "姓名")]
        public string PaymentRealName { get; set; }

        [Required]
        [Display(Name = "身份证")]
        [RegularExpression(@"(^\d{15}$)|(^\d{17}([0-9]|X)$)", ErrorMessage = "*请输入正确的格式")]
        public string PaymentIdCard { get; set; }

        [Required]
        [Display(Name="支付密码")]
        [DataType(DataType.Password)]
        [StringLength(6,MinimumLength=6,ErrorMessage="*必须为6位字符")]
        public string PaymentPayPssword { get; set; }
   

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "余额")]
        public decimal PaymentBalance { get; set; }

        [Required]
        [RegularExpression(@"^(\d{16}|\d{19})$", ErrorMessage = "*请输入正确的格式")]
        [Display(Name = "银行卡号")]
        public string PaymentBankCard { get; set; }

        [Required]
        [Display(Name = "会员名")]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; } 

    }
    [Table("db_PaymentLog")]
    public class PaymentLog
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int PaymentLogId { get; set; }

        [Display(Name = "交易时间")]
        [DataType(DataType.Time)]
        [ScaffoldColumn(false)]
        public DateTime PaymentLogTime { get; set; }

        [Required]
        [Display(Name = "交易名称")]    //HTC M7 白色
        [StringLength(20, ErrorMessage = "*不能超过20个字符")]
        public string PaymentLogName { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "交易金额")]
        public decimal PaymentLogPrice { get; set; }

        [Required]
        [Display(Name = "支付通")]
        public int PaymentId { get; set; }

        public virtual Payment Payment { get; set; }

    }

    [Table("db_OrderStatus")]
    public class OrderStatus
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int OrderStatusId { get; set; }

        [Required]
        [Display(Name = "状态名称")]
        [StringLength(20)]
        public string OrderStatusIdName { get; set; }

    }

    [Table("db_Order")]
    public class Order
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "订单号")]  
        public string OrderNumber { get; set; }


        [Required]
        [Display(Name = "下单时间")]
        [StringLength(100)]
        [DataType(DataType.Time)]
        public string OrderTime { get; set; }
      
        [Display(Name = "支付时间")]
        [DataType(DataType.Time)]
        [StringLength(100)]
        [ScaffoldColumn(false)]
        public string OrderPayTime { get; set; }

        [Display(Name = "发货时间")]
        [DataType(DataType.Time)]
        [StringLength(100)]
        [ScaffoldColumn(false)]
        public string OrderSendTime { get; set; }

        [Display(Name = "成交时间")]
        [DataType(DataType.Time)]
        [StringLength(100)]
        [ScaffoldColumn(false)]
        public string OrderSuccessTime { get; set; }


        [Required]
        [Display(Name = "单价/元")]
        [DataType(DataType.Currency)]
        public decimal OrderPrice { get; set; }

        [Required]
        [Display(Name="数量")]
        public int OrderCount { get; set; }

        [Required]
        [StringLength(500)]
        [ScaffoldColumn(false)]
        public string AddressDetails { get; set; }//联系人，电话好吗，地址名称,邮编,

        [Required]
        [StringLength(20)]
        [Display(Name = "产品型号")]
        public string OrderProName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "产品介绍")]
        public string OrderProIntroduce { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "产品封面")]
        public string OrderProCoverImage { get; set; }

        [Required]
        [Display(Name = "产品颜色")]
        public string ColorName { get; set; }

        [Required]
        [Display(Name = "产品编号")]
        public int ProId { get; set; }

        [Required]
        [Display(Name = "库存编号")]
        public int StorageId { get; set; }

        [Required]
        [Display(Name = "状态")]
        public int OrderStatusId { get; set; }
       
        public virtual OrderStatus OrderStatus { get; set; }


        [Required]
        [Display(Name = "会员名")]
        public int MemberId { get; set; }

        public virtual Member Member { get; set; } 

    }

    [Table("db_Cart")]
    public class Cart
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int CartId { get; set; }

        [Required]
        [Display(Name = "加入购物车时间")]
        [DataType(DataType.Time)]
        public DateTime CartTime { get; set; }

        [Required]
        [Display(Name = "数量")]
        public int CartCount { get; set; }

        [Required]
        [Display(Name = "产品编号")]
        public int ProId { get; set; }

        [Required]        
        [Display(Name = "库存编号")]
        public int StorageId { get; set; }

        [Required]
        [Display(Name = "颜色编号")]
        public int ColorId { get; set; }

        [Required]
        [Display(Name = "会员名")]
        public int MemberId { get; set; }


        public virtual ProductColor ProductColor { get; set; }   
     
        public virtual ProductInfo ProductInfo { get; set; }             
        
        public virtual Member Member { get; set; }

    }

    public class CheckOrder
    {
        public int ProId { get; set; }
        public int StorageId { get; set; }
        public int ColorId { get; set; }
        public string ProName { get; set; }
        public decimal ProPrice { get; set; }
        public int ProNum { get; set; }
        public int StorageNum { get; set; }

    }


    [Table("db_Navigations")]
    public class Navigation
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int NavId { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "*字符长度不能大于8个！")]
        [Display(Name = "导航名称")]
        [System.Web.Mvc.Remote("IsExistNavigation", "Navigation", ErrorMessage = "该导航名已存在！")]  
        public string NavName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "导航网址")]
        public string NavUrl { get; set; }
    }

    [Table("db_Slides")]
    public class Slide
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int SidId { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "幻灯片图片")]
        public string SidImgUrl { get; set; }   //推荐大小1000*400

        [Required]
        [StringLength(100)]
        [Display(Name = "幻灯片网址")]
        public string SidUrl { get; set; }     //前台页面中的连接或外链。        
    }

}