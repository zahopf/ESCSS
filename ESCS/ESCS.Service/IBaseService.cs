using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using ESCS.Domain;
using ESCS.Manager;

namespace ESCS.Service
{
    public interface IBaseService<T> where T : EntityBase, new() //约束条件：T只能是继承自EntityBase的实体
    {
        void Create(T entity);//创建实体

        void Update(T entity);//更新实体

        T Get(int ID);//根据ID获取实体

        T Get(IList<ICriterion> queryConditions);//根据条件获取一个实体

        void Delete(int ID);//根据ID删除实体

        void Delete(T entity);//根据实体删除

        bool Exists(int id);//根据id判断数据是否存在
        bool Exists(IList<ICriterion> queryConditions);//根据查询条件判断数据是否存在

        IList<T> GetAll();//获取所有

        IList<T> GetAll(IList<ICriterion> queryConditions);//通用查询方法，获取满足条件的记录

        /// <summary>
        /// 分页获取满足条件的记录
        /// </summary>
        /// <param name="queryConditions">条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="orderField"></param>
        /// <param name="isAscending">升序 true,降序 false</param>
        /// <param name="count">数量集合数量</param>
        /// <returns>满足条件的实体</returns>
        IList<T> GetPaged(IList<ICriterion> queryConditions, IList<Order> orderList, int pageIndex, int pageSize, out int count);

        /// <summary>
        /// 获取满足条件的记录
        /// </summary>
        /// <param name="queryConditions">分页查询条件，key对应查询对象，如:TransportOrder.Receiver.Name
        /// value对应查询对象对应的值：如“李强”，key和value加并后代码TransportOrder.Receiver.Name="李强"</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="count">满足当前条件的记录数</param>
        /// <returns>满足查询条件的实体集合</returns>
        IList<T> GetPaged(IList<QueryConditions> queryConditions, int pageIndex, int pageSize, out int count);

        /// <summary>
        /// 根据查询条件查询满足条件的记录
        /// </summary>
        /// <param name="queryConditions">查询条件</param>
        /// <returns>满足查询条件的实体集合</returns>
        IList<T> Find(IList<ICriterion> queryConditions);
    }
}
