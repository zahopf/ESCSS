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
    [ActiveRecord("PurchaseOrder")]
    public class PurchaseOrder : EntityBase
    {
        //数量
        [Property(NotNull = true)]
        public int Quantity { get; set; }

        //时间
        [Property(NotNull = true)]
        public DateTime Time { get; set; }

        //预计金额
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "Money", Description = "金额")]
        public double Money1 { get; set; }

        //实际金额
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "Money", Description = "金额")]
        public double Money2 { get; set; }

        //状态
        [Property(NotNull = true)]
        [Display(Name = "状态")]
        public virtual string Status { get; set; }

        //备注
        [Property(NotNull = false)]
        [Display(Name = "备注")]
        public virtual string Remarks{ get; set; }

        //进货人
        [BelongsTo(Type = typeof(User), Column = "UserId", Lazy = FetchWhen.OnInvoke)]
        public User UserId { get; set; }

        //订单生成人
        [BelongsTo(Type = typeof(User), Column = "UserIds", Lazy = FetchWhen.OnInvoke)]
        public User UserIds { get; set; }

        //进货单明细
        [HasMany(typeof(PurchaseOrderDetails), Table = "PurchaseOrderDetails", ColumnKey = "PurchaseOrderId", Cascade = ManyRelationCascadeEnum.All, Lazy = false, Inverse = false)]
        public IList<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    }
}
