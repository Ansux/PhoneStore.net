using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PhoneStoreSystem.Models;

namespace PhoneStoreSystem.Filters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class LoginNameUniqueAttribute : ValidationAttribute
    {
        private PhoneStoreContext db = new PhoneStoreContext();

        private static readonly string DefaultErrorMessage = "该字段值不可重复！";//MUI.login_unique;

        public LoginNameUniqueAttribute()
            : base(DefaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return DefaultErrorMessage;
        }

        public override bool IsValid(object value)
        {
            string v = value.ToString();
            if (!string.IsNullOrEmpty(v))
            {
                var u = from n in db.UserInfos
                        where n.UserName == v
                        select n;;
                if (u.Count()!=0)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
    

    



}