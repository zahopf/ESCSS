using ESCS.Domain;
using ESCS.Manager;
using ESCS.Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ESCS.Component
{
    public class GoodsComponent:BaseComponent<Goods,GoodsManager>,IGoodsService
    {
        /// <summary>
        /// 交换商品状态
        /// </summary>
        /// <param name="id"></param>
        public void SwitchStatus(int id)
        {
            Goods goods = this.Get(id);//根据id获取goods实体
            goods.IsActive = !goods.IsActive;//设置goods的状态为“非”当前状态，即如果当前是活动的则设置为不活动，如果当前是不活动的则设置为活动
            Update(goods);//更新当前用户
        }
        
    }
}
