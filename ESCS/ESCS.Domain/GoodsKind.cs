using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;

namespace ESCS.Domain
{
    [ActiveRecord]
    public class GoodsKind : EntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        /// 调用EntityBase类实现

        [Property( NotNull = true)]
        [Required(ErrorMessage = "不能为空")]
        public string KindName { get; set; }

        [HasMany(typeof(Goods), Table = "[Goods]", ColumnKey = "GoodsId", Cascade = ManyRelationCascadeEnum.SaveUpdate, Lazy = true, Inverse = false)]
        public virtual IList<Goods> GoodsList { get; set; }

        //父亲ID
        [BelongsTo(Type = typeof(GoodsKind), Column = "ParentID", NotNull = false)]
        public virtual GoodsKind ParentID { get; set; }
    }

}
