using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using System.ComponentModel.DataAnnotations;

namespace ESCS.Domain
{
    public class EntityBase
    {
        [PrimaryKey(PrimaryKeyType.Identity)]//主键的类型为自增型
        [Display(AutoGenerateField = false)]//在生成视图时不生成本属性
        public virtual int ID { get; set; }//主键

    }
}
