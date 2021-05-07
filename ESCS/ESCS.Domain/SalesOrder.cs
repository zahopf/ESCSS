using Castle.ActiveRecord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Domain
{
    [ActiveRecord("SalesOrder")]
    public class SalesOrder:EntityBase
    {
        [Property(NotNull = true)]
        [Display(Name = "订单生成时间")]
        public virtual DateTime Time { get; set; }

        //数量
        [Property(NotNull = true)]
        public int Quantity { get; set; }

        //金额
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "总金额")]
        public double Money { get; set; }

        //利润
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "利润")]
        public double Profits { get; set; }


        //状态
        [Property(NotNull = true)]
        [Display(Name = "状态")]
        public virtual string Status { get; set; }

        //备注
        [Property(NotNull = false)]
        [Display(Name = "备注")]
        public virtual string Remarks { get; set; }

        //进货人
        [BelongsTo(Type = typeof(User), Column = "UserId", Lazy = FetchWhen.OnInvoke)]
        public User UserId { get; set; }

        //客户
        [BelongsTo(Type = typeof(Customer), Column = "CustomerId", Lazy = FetchWhen.OnInvoke)]
        public Customer CustomerId { get; set; }

        //收货地址
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "收货地址", Description = "收货地址")]
        public virtual string Addressing { get; set; }

        //收件人
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "收件人", Description = "收件人")]
        public virtual string Addressee { get; set; }

        //售货单明细
        [HasMany(typeof(SalesOrderDetails), Table = "SalesOrderDetails", ColumnKey = "SalesOrderId", Cascade = ManyRelationCascadeEnum.All, Lazy = false, Inverse = false)]
        public IList<SalesOrderDetails> SalesOrderDetails { get; set; }
    }
}
