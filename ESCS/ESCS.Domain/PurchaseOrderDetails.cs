using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;
using NHibernate.Criterion;
using System.Web.Mvc;
using System;


namespace ESCS.Domain
{
    [ActiveRecord("PurchaseOrderDetails")]
    public class PurchaseOrderDetails : EntityBase
    {
        //数量
        [Property(NotNull = true)]
        public int InnQuantity { get; set; }

        //商品
        [BelongsTo(Type = typeof(Goods), Column = "GoodsId", Lazy = FetchWhen.OnInvoke)]
        public Goods GoodsId { get; set; }

        //单号
        [BelongsTo(Type = typeof(PurchaseOrder), Column = "PurchaseOrderId", Lazy = FetchWhen.OnInvoke)]
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
