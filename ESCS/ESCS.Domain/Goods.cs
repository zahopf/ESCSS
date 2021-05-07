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
    [ActiveRecord("Goods")]
    public class Goods:EntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        /// 调用EntityBase类实现

        //商品名字
        [Property( NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [Display(Name = "商品名字")]
        public virtual string GoodsName { get; set; }

        //图片
        [Property(NotNull = true)]
        [Display(Name = "图片")]
        public virtual string Picture { get; set; }

        //折扣
        [Property(NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [Range(0.1, 1,ErrorMessage="折扣必须在0.1-1之间")] 
        [Display(Name = "折扣")]
        public float Discount { get; set; }

        //进货价格
        [Property(NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [Display(Name = "进价")]
        public virtual float InnPrice { get; set; }

        //出货价格
        [Property(NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [Display(Name = "售价")]
        public virtual float OutPrice { get; set; }

        //库存
        [Property(NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        [Display(Name = "库存")]
        public virtual int Stock { get; set; }

        [BelongsTo(Type = typeof(GoodsKind), Column = "GoodsKindId", Lazy = FetchWhen.OnInvoke)]
        public virtual GoodsKind GoodsKindId { get; set; }


        [Property(NotNull = true)]
        [Display(Name = "激活")]
        public virtual bool IsActive { get; set; }

        [Property(NotNull = true)]
        [Display(Name = "状态")]
        public virtual string Status { get; set; }

        ////进货单明细
        //[HasMany(typeof(PurchaseOrderDetails), Table = "PurchaseOrderDetails", ColumnKey = "PurchaseOrderId", Cascade = ManyRelationCascadeEnum.All, Lazy = false, Inverse = false)]
        //public IList<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }


    }
}
