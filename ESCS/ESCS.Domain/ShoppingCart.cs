using Castle.ActiveRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Domain
{
    [ActiveRecord("ShoppingCart")]
    public class ShoppingCart : EntityBase
    {
        //数量
        [Property(NotNull = true)]
        public int Quantity { get; set; }

        //商品
        [BelongsTo(Type = typeof(Goods), Column = "GoodsId", Lazy = FetchWhen.OnInvoke)]
        public Goods GoodsId { get; set; }

        //客户
        [BelongsTo(Type = typeof(Customer), Column = "CustomerId", Lazy = FetchWhen.OnInvoke)]
        public Customer CustomerId { get; set; }
    }
}
