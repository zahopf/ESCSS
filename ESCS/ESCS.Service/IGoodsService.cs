using ESCS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ESCS.Service
{
    public interface IGoodsService:IBaseService<Goods>
    {
        /// <summary>
        /// 根据id修改实体状态
        /// </summary>
        /// <param name="id">要修改对象的id</param>
        void SwitchStatus(int id);
    }

}
