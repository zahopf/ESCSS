using ESCS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Service
{
    public interface ISalesOrderService : IBaseService<SalesOrder>
    {
        /// <summary>
        /// 时间格式判断
        /// </summary>
        /// <param name="time">订单集合</param>
        /// <returns></returns>
        bool Time(string time);

        /// <summary>
        /// 查询时间间隔内订单数
        /// </summary>
        /// <param name="hitime">时间</param>
        /// <param name="lotime">时间</param>
        /// <returns></returns>
        IList<SalesOrder> BetweenTime(string hitime, string lotime);
    }
}
