using Castle.ActiveRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Domain
{
    [ActiveRecord("SalesOrderDetails")]
    public class SalesOrderDetails : EntityBase
    {
        //数量
        [Property(NotNull = true)]
        public int Quantity { get; set; }

        //商品
        [BelongsTo(Type = typeof(Goods), Column = "GoodsId", Lazy = FetchWhen.OnInvoke)]
        public Goods GoodsId { get; set; }

        //单号
        [BelongsTo(Type = typeof(SalesOrder), Column = "SalesOrderId", Lazy = FetchWhen.OnInvoke)]
        public SalesOrder SalesOrder { get; set; }
    }
}
