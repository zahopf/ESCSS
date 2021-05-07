using ESCS.Domain;
using ESCS.Manager;
using ESCS.Service;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Component
{
    public class SalesOrderComponent : BaseComponent<SalesOrder, SalesOrderManager>, ISalesOrderService
    {
        /// <summary>
        /// 判断时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Time(string time)
        {
            if (time.Length == 16 && time[4].ToString() == "/" && time[7].ToString() == "/" && time[10].ToString() == " " && time[13].ToString() == ":")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 查询时间间隔内订单数
        /// </summary>
        /// <param name="hitime">时间</param>
        /// <param name="lotime">时间</param>
        /// <returns></returns>
        public IList<SalesOrder> BetweenTime(string hitime, string lotime)
        {
            IList<Order> listOrder = new List<Order>() { new Order("ID", true) };//设置一个排序集合
            int count = 0;
            DateTime Hitime = DateTime.Parse(hitime);
            DateTime Lotime = DateTime.Parse(lotime);
            IList<ICriterion> queryConditions = new List<ICriterion>();
            queryConditions.Add(Expression.Between("Time", Lotime, Hitime));

            //查询指定索引页的的商品信息
            IList<SalesOrder> list = manager.GetAll().Where(o => o.Status == "完成").ToList().Where(o => o.Remarks == "购买").ToList();
            return list;
        }
    }
}
