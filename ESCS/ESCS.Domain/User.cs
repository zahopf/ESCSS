using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.ActiveRecord;

using System.ComponentModel.DataAnnotations;

namespace ESCS.Domain
{
    [ActiveRecord("SysUser")]
    public class User : EntityBase
    {
        //客户电话号码
        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [StringLength(15, ErrorMessage = "长度不能操作15.")]
        [Display(Name = "Number", Description = "客户电话号码")]
        public virtual string Number { get; set; }

        //邮箱
        [Property(NotNull = true, Length = 50)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [StringLength(20, ErrorMessage = "不能超过50个字符")]
        [Display(Name = "邮箱")]
        public virtual string Email { get; set; }


        [Property(NotNull = true)]
        [Display(Name = "图片")]
        public virtual string Picture { get; set; }

        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [StringLength(20, ErrorMessage = "不能超过20个字符")]
        [Display(Name = "用户名")]
        public virtual string UserName { get; set; }

        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [StringLength(20, ErrorMessage = "不能超过20个字符")]
        [Display(Name = "性别")]
        public virtual string Sex { get; set; }

        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [Display(Name = "年龄", Description = "年龄")]
        public virtual int Age { get; set; }

        //账号
        [Property(NotNull = true, Length = 50)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [StringLength(20, ErrorMessage = "不能超过50个字符")]
        [Display(Name = "帐号")]
        public virtual string Account { get; set; }


        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        public virtual string Password { get; set; }

        //状态
        [Property(NotNull = true)]
        [Display(Name = "激活")]
        public virtual bool IsActive { get; set; }


        [Property(NotNull = true)]
        [Required(ErrorMessage = "必须填写字段信息哦!!!")]
        [Display(Name = "工资")]
        public virtual float Salary { get; set; }

        //角色
        [HasAndBelongsToMany(typeof(Role), 
            Table = "User_Role", 
            ColumnKey = "UserId", 
            ColumnRef = "RoleId", 
            Cascade = ManyRelationCascadeEnum.None, 
            Inverse = false)]
        public virtual IList<Role> RoleList { get; set; }
      
    }
}
