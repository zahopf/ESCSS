using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using NHibernate.Criterion;
using System.ComponentModel.DataAnnotations;
using NHibernate;
using ESCS.Domain;

namespace ESCS.Manager
{
    /// <summary>
    /// 实体类基类，所有实体必须继承自EntityBase（为了被ORM框架管理）
    /// </summary>
    /// <typeparam name="T">子类的类型，如：Driver,User</typeparam>
    public class ManagerBase<T> : ActiveRecordBase<T>
        where T : class
    {
        public new void Create(T t)//创建t
        {
            ActiveRecordBase.CreateAndFlush(t);//在数据库中持久化t，并同步Session
        }

        public new void Update(T t)//更新数据库中的t
        {
            ActiveRecordBase.UpdateAndFlush(t);//更新数据库中的t，并同步Session
        }

        public new void DeleteAll()//删除T类对应的表中的所有数据
        {
            DeleteAll(typeof(T));
        }

        public void Delete(int id)//根据id删除实体
        {
            T t = Get(id);//根据id获取对象
            if (t != null)//如果对象存在
            {
                ActiveRecordBase.DeleteAndFlush(t);//删除它
            }
        }

        public new void Delete(T t)//删除t对象
        {
            ActiveRecordBase.DeleteAndFlush(t);//删除数据库中的t，并同步Session
        }

        public bool Exists(int id)//根据id判断T对应数据库表中是否存在ID为id的数据
        {
            return ActiveRecordBase.Exists(typeof(T), id);
        }

        public bool Exists(IList<ICriterion> queryConditions)//根据查询条件判断数据库中是否存在数据
        {
            return ActiveRecordBase.Exists(typeof(T), queryConditions.ToArray());
        }

        public IList<T> GetAll()//获取所有对象
        {
            return FindAll(typeof(T)) as IList<T>;
        }

        public IList<T> GetAll(IList<ICriterion> queryConditions)//根据查询条件集合获取所有对象
        {
            Array arr = FindAll(typeof(T), queryConditions.ToArray());//Castle.net默认是数组
            return arr as IList<T>;//将数组转换为IList集合
        }

        public T Get(int ID)//根据ID获取对象
        {
            return FindByPrimaryKey(typeof(T), ID) as T;
        }

        public T Get(IList<ICriterion> queryConditions)//根据查询条件查询一条数据
        {
            object obj = ActiveRecordBase.FindOne(typeof(T), queryConditions.ToArray());
            if (obj == null) return null;
            else return obj as T;
        }

        public IList<T> GetAll(IList<QueryConditions> queryConditions)//构造通用条件查询
        {
            //实例化一个hql查询语句对象
            StringBuilder hql = new StringBuilder(@"from " + typeof(T).Name + " d");
            //根据查询条件构造hql查询语句
            for (int i = 0; i < queryConditions.Count; i++)
            {
                QueryConditions qc = queryConditions[i];//获取当前序号对应的条件
                if (qc.Value != null)
                {
                    AddHqlSatements(hql);//增加where或and语句
                    hql.Append(string.Format("d.{0} {1} :q_{2}", qc.PropertyName, qc.Operator, i));
                }
            }

            ISession session = ActiveRecordBase.holder.CreateSession(typeof(T));//获取管理T的session对象
            IQuery query = session.CreateQuery(hql.ToString());//获取满足条件的数据
            IQuery queryScalar = session.CreateQuery("select count(ID) " + hql.ToString());//获取满足条件的数据的总数
            for (int i = 0; i < queryConditions.Count; i++)
            {
                QueryConditions qc = queryConditions[i];//获取当前序号对应的条件
                if (qc.Value != null)
                {
                    //如果是like语句，则修改值的表达方式
                    if (qc.Operator.ToUpper() == "LIKE")
                    {
                        qc.Value = "%" + qc.Value + "%";
                    }

                    //用查询条件的值去填充hql，如d.Transportor.Name="michael"
                    queryScalar.SetParameter("q_" + i, qc.Value);
                    query.SetParameter("q_" + i, qc.Value);
                }
            }

            IList<object> result = queryScalar.List<object>();//执行查询条件总数的查询对象，返回一个集合（有一点怪异）

            IList<T> arr = query.List<T>();//返回当前页的数据
            //session.Close(); //此不要要显示关闭，因为在系统的HttpModle中已经统一处理
            return arr;
        }

        //分页区和取对象集合
        public IList<T> GetPaged(IList<ICriterion> queryConditions, IList<Order> orderList, int pageIndex, int pageSize, out int count)
        {
            if (queryConditions == null)//如果为null则赋值为一个总数为0的集合
            {
                queryConditions = new List<ICriterion>();
            }
            if (orderList == null)//如果为null则赋值为一个总数为0的集合
            {
                orderList = new List<Order>();
            }
            count = Count(typeof(T), queryConditions.ToArray());//根据查询条件获取满足条件的对象总数
            Array arr = SlicedFindAll(typeof(T), (pageIndex - 1) * pageSize, pageSize, orderList.ToArray(), queryConditions.ToArray());//根据查询条件分页获取对象集合
            return arr as IList<T>;
        }

        /// <summary>
        /// 根据查询条件分页获取实体
        /// </summary>
        /// <param name="queryConditions">查询条件集合</param>
        /// <param name="pageIndex">当前页码，从1开始</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="count">返回满足查询条件</param>
        /// <returns>返回满足查询条件的实体</returns>
        public IList<T> GetPaged(IList<QueryConditions> queryConditions, int pageIndex, int pageSize, out int count)
        {
            //实例化一个hql查询语句对象
            StringBuilder hql = new StringBuilder(@"from " + typeof(T).Name + " d");
            //根据查询条件构造hql查询语句
            for (int i = 0; i < queryConditions.Count; i++)
            {
                QueryConditions qc = queryConditions[i];//获取当前序号对应的条件
                if (qc.Value != null)
                {
                    AddHqlSatements(hql);//增加where或and语句
                    hql.Append(string.Format("d.{0} {1} :q_{2}", qc.PropertyName, qc.Operator, i));
                }
            }

            ISession session = ActiveRecordBase.holder.CreateSession(typeof(T));//获取管理T的session对象
            IQuery query = session.CreateQuery(hql.ToString());//获取满足条件的数据
            IQuery queryScalar = session.CreateQuery("select count(ID) " + hql.ToString());//获取满足条件的数据的总数
            for (int i = 0; i < queryConditions.Count; i++)
            {
                QueryConditions qc = queryConditions[i];//获取当前序号对应的条件
                if (qc.Value != null)
                {
                    //如果是like语句，则修改值的表达方式
                    if (qc.Operator.ToUpper() == "LIKE")
                    {
                        qc.Value = "%" + qc.Value + "%";
                    }

                    //用查询条件的值去填充hql，如d.Transportor.Name="michael"
                    queryScalar.SetParameter("q_" + i, qc.Value);
                    query.SetParameter("q_" + i, qc.Value);
                }
            }

            IList<object> result = queryScalar.List<object>();//执行查询条件总数的查询对象，返回一个集合（有一点怪异）
            int.TryParse(result[0].ToString(), out count);//尝试将返回值的第一个值转换为整形，并将转换成功的值赋值给count，如果转换失败,count=0
            query.SetFirstResult((pageIndex - 1) * pageSize);//设置获取满足条件实体的起点
            query.SetMaxResults(pageSize);//设置获取满足条件实体的终点
            IList<T> arr = query.List<T>();//返回当前页的数据
            //session.Close(); //此不要要显示关闭，因为在系统的HttpModle中已经统一处理
            return arr;
        }

        protected void AddHqlSatements(StringBuilder hql)
        {
            if (!hql.ToString().Contains("where"))//查询语句的开始条件是where
            {
                hql.Append(" where ");
            }
            else//当hql中有了一个where后再添加查询条件时就应该使用and了
            {
                hql.Append(" and ");
            }
        }

        /// <summary>
        /// 根据条件查找数据，返回集合
        /// </summary>
        /// <param name="queryConditions"></param>
        /// <returns></returns>
        public IList<T> Find(IList<ICriterion> queryConditions)
        {
            Array arr = ActiveRecordBase.FindAll(typeof(T), queryConditions.ToArray());
            return arr as IList<T>;
        }

    }

    /// <summary>
    /// 查询条件
    /// </summary>
    public class QueryConditions
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 操作符号：= > < >= <= != isEmpty like等 
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
    }
}
