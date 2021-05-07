using Castle.ActiveRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace ESCS.Domain
{
    [ActiveRecord("SysFunction")]
    public class SystemFunction:EntityBase
    {
        /// <summary>
        /// 系统功能类
        /// </summary>
        /// 
        //系统功能名称
        [Display(Name = "系统功能名称")]
        [Property(Length = 20, NotNull = false)]
        public virtual string SystemFunctionName { get; set; }

        //类名
        [Display(Name = "类名")]
        [Property(Length = 200, NotNull = false)]
        public virtual string ClassName { get; set; }

        //控制器名称
        [Display(Name = "控制器名称")]
        [Property(Length = 50, NotNull = false)]
        public virtual string ControllerName { get; set; }

        //Action名称
        [Display(Name = "Action名称")]
        [Property(Length = 200, NotNull = false)]
        public virtual string ActionName { get; set; }

        //排序索引
        [Display(Name = "排序索引")]
        [Property(Length = 4, NotNull = false)]
        public virtual int Idx { get; set; }

        //是否展示
        [Display(Name = "是否展示")]
        [Property(NotNull = false)]
        public virtual bool IsShow { get; set; }

        //父亲ID
        [BelongsTo(Type = typeof(SystemFunction), Column = "ParentID",NotNull=false)]
        public virtual SystemFunction ParentID { get; set; }

        [HasAndBelongsToMany(typeof(Role), Table = "Role_SysFunction", ColumnKey = "SysFunctionId", ColumnRef = "RoleId", Cascade = ManyRelationCascadeEnum.None, Inverse = false)]
        public virtual IList<Role> RoleList { get; set; }
    }
}
