using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;
using NHibernate.Criterion;
using System.Web.Mvc;

namespace ESCS.Domain
{
    [ActiveRecord("Address")]
    public class Address : EntityBase
    {
        //状态（默认地址？默认=true）
        [Property(NotNull = true)]
        [Display(Name = "默认")]
        public virtual bool IsActive { get; set; }


        //客户
        [BelongsTo(Type = typeof(Customer), Column = "CustomerId", Lazy = FetchWhen.OnInvoke)]
        public Customer CustomerId { get; set; }

        //地址
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "地址", Description = "地址")]
        public virtual string Addressing { get; set; }

        //详细地址
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "详细地址", Description = "详细地址")]
        public virtual string AddressingDetail { get; set; }

        //收件人
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "收件人", Description = "收件人")]
        public virtual string Addressee { get; set; }

        
    }
}
