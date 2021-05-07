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
    [ActiveRecord("Customer")]
    public class Customer:EntityBase
    {
        //客户名
        [Property(NotNull = true)]
        [StringLength(20, ErrorMessage = "长度不能操作20.")]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "CustomerName", Description = "客户名")]
        public virtual string CustomerName { get; set; }

        //是否为VIP
        [Property(NotNull = true)]
        public virtual bool VIP { get; set; }

        

        //积分（一元一积分）
        [Property]
        [Display(Name = "Integral", Description = "积分（一元一积分")]
        public virtual int Integral { get; set; }

        [Property(NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(20, ErrorMessage = "不能超过20个字符")]
        [Display(Name = "性别")]
        public virtual string Sex { get; set; }

        [Property(NotNull = true)]
        [Display(Name = "Birthday", Description = "生日")]
        public virtual DateTime Birthday { get; set; }

        //账号
        [Property(NotNull = true, Length = 50)]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(20, ErrorMessage = "不能超过50个字符")]
        [Display(Name = "帐号")]
        public virtual string Account { get; set; }

       
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        public virtual string Password { get; set; }

        //状态
        [Property(NotNull = true)]
        [Required]
        [Display(Name = "激活")]
        public virtual bool IsActive { get; set; }

        //客户电话号码
        [Property(NotNull = true)]
        [StringLength(15, ErrorMessage = "长度不能操作15.")]
        [Display(Name = "Number", Description = "客户电话号码")]
        public virtual string Number { get; set; }

        //邮箱
        [Property(NotNull = true, Length = 50)]
        [Required(ErrorMessage = "不能为空")]
        [StringLength(20, ErrorMessage = "不能超过50个字符")]
        [Display(Name = "邮箱")]
        public virtual string Email { get; set; }


        [Property(NotNull = true)]
        [Display(Name = "图片")]
        public virtual string Picture { get; set; }
    }
}
