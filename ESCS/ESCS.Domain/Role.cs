using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;

namespace ESCS.Domain
{
    [ActiveRecord("Role")]
    public class Role : EntityBase
    {
        [Property(NotNull = true)]
        [StringLength(20, ErrorMessage = "长度不能超过20.")]
        [Required(ErrorMessage = "必须填写字段信息哦。")]
        [Display(Name = "角色名")]
        public virtual string RoleName { get; set; }

        [Property(NotNull = true)]
        [Required]
        [Display(Name = "激活")]
        public bool IsActive { get; set; }

        [HasAndBelongsToMany(typeof(User), Table = "User_Role", ColumnKey = "RoleId", ColumnRef = "UserId", Cascade = ManyRelationCascadeEnum.None, Inverse = false)]
        public virtual IList<User> UserList { get; set; }

        [HasAndBelongsToMany(typeof(SystemFunction), Table = "Role_SysFunction", ColumnKey = "RoleId", ColumnRef = "SysFunctionId", Cascade = ManyRelationCascadeEnum.None, Inverse = false)]
        public virtual IList<SystemFunction> SysFunctionList { get; set; }

    }
}
