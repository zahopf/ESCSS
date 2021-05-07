using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESCS.Service;
using Castle.ActiveRecord;
using ESCS.Domain;
using NHibernate.Criterion;
using Castle.Services.Transaction;
using ESCS.Manager;

namespace ESCS.Component
{
    public class BaseComponent<T, M> : IBaseService<T> //实现IBaseService接口
        where T : EntityBase, new()//约束条件：T只能是继承自EntityBase的实体
        where M : ManagerBase<T>, new()//约束条件：M只能是继承自ManagerBase的管理类
    {
        protected M manager =  (M)typeof(M).GetConstructor(Type.EmptyTypes).Invoke(null);
        public virtual void Create(T entity)
        {
            manager.Create(entity);
        }

        public virtual void Update(T entity)
        {
            manager.Update(entity);
        }
        
        public T Get(int ID)
        {
            return manager.Get(ID);
        }

        public T Get(IList<ICriterion> queryConditions)
        {
            return manager.Get(queryConditions);
        }

        public void Delete(int ID)
        {
            manager.Delete(ID);
        }

        public void Delete(T entity)
        {
            manager.Delete(entity);
        }

        public bool Exists(int id)
        {
            return manager.Exists(id);
        }

        public bool Exists(IList<ICriterion> queryConditions)
        {
            return manager.Exists(queryConditions);
        }

        public IList<T> GetAll()
        {
            return manager.GetAll();
        }

        public IList<T> GetAll(IList<NHibernate.Criterion.ICriterion> queryConditions)
        {
            return manager.GetAll(queryConditions);
        }

        public IList<T> GetPaged(IList<ICriterion> queryConditions, IList<Order> orderList, int pageIndex, int pageSize, out int count)
        {
            return manager.GetPaged(queryConditions, orderList, pageIndex, pageSize, out count);
        }

        public IList<T> Find(IList<ICriterion> queryConditions)
        {
            return manager.Find(queryConditions);
        }

        public IList<T> GetPaged(IList<QueryConditions> queryConditions, int pageIndex, int pageSize, out int count)
        {
            return manager.GetPaged(queryConditions, pageIndex, pageSize, out count);
        }

        public IList<T> Find(IList<QueryConditions> queryConditions)
        {
            return manager.GetAll(queryConditions);
        }
    }
}
