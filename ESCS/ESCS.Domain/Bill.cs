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
    [ActiveRecord("Bill")]
    public class Bill : EntityBase
    {
        //备注
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "Remarks", Description = "备注")]
        public virtual string Remarks { get; set; }

        //时间
        [Property(NotNull = true)]
        public DateTime Time { get; set; }

        //金额
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "Money", Description = "金额")]
        public virtual double Money { get; set; }

        //余额
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "Balance", Description = "余额")]
        public virtual double Balance { get; set; }

        //操作人
        [BelongsTo(Type = typeof(User), Column = "UserId", Lazy = FetchWhen.OnInvoke)]
        public User UserId { get; set; }
    }
}
