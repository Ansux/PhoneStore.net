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
    /*
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("PhoneStoreDBConnection")
        {
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<RoleInfo> RoleInfos { get; set; }
        
    }
    */

    [Table("db_UserInfo")]
    public class UserInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 2)]
        [Display(Name = "用户名")]
        //[LoginNameUnique]
        [System.Web.Mvc.Remote("IsExistUser","Account",ErrorMessage="该用户名已存在！")]  
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(18, MinimumLength = 6)]
        [Display(Name = "密码")]
        public string PassWord { get; set; }

        [Required]
        [Display(Name = "角色")]
        public int RoleId { get; set; }

       
        [Display(Name = "真实姓名")]
        [StringLength(20)]
        public string RealName { get; set; }

       
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确的邮箱地址")]
        [Display(Name = "邮箱")]
        public string UserEmail { get; set; }

        public virtual RoleInfo RoleInfo { get; set; }

    }

    public class UserProfile
    {
        public int UserId { get; set; }

        [Display(Name = "真实姓名")]
        [StringLength(20)]
        public string RealName { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确的邮箱地址")]
        [Display(Name = "邮箱")]
        public string UserEmail { get; set; }
    }

    [Table("db_RoleInfo")]
    public class RoleInfo {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "角色名")]
        public string RoleName { get; set; }


    }

    public class LoginModel
    {
        [Required(ErrorMessage="*")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "{0} 必须为{2}-{1} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        //[LoginNameUnique]
        [System.Web.Mvc.Remote("IsExistUser", "Account", ErrorMessage = "xxxxxxx")]  
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "真实姓名")]
        [StringLength(20)]
        public string RealName { get; set; }


        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "请输入正确的邮箱地址")]
        [Display(Name = "邮箱")]
        public string UserEmail { get; set; }
    }
}