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
    public class UserComponent:BaseComponent<User,UserManager>,IUserService
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">操作员输入的用户名</param>
        /// <param name="password">操作员输入的密码</param>
        /// <returns>当用户名和密码成功匹配时返回匹配的用户信息，否则返回null</returns>
        public User Login(string account, string password)
        {
            //组织查询条件(标准条件查询)
            IList<NHibernate.Criterion.ICriterion> criterionList = new List<ICriterion>();
            //向条件集合添加查询条件，每个条件之间默认为And关系，从数据库中查询同时满足3个条件的用户信息
            criterionList.Add(Expression.Eq("Account", account));
            criterionList.Add(Expression.Eq("Password", password));
            criterionList.Add(Expression.Eq("IsActive", true));

            //调用数据访问层的方法执行操作       
            User user = manager.Get(criterionList);
            return user;
        }

        /// <summary>
        /// 交换用户状态
        /// </summary>
        /// <param name="id"></param>
        public void SwitchStatus(int id)
        {
            User user = this.Get(id);//根据id获取user实体
            user.IsActive = !user.IsActive;//设置user的状态为“非”当前状态，即如果当前是活动的则设置为不活动，如果当前是不活动的则设置为活动
            Update(user);//更新当前用户
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="rolesId">角色ID串，中间用“,”隔开</param>
        public void Create(User user, string rolesId)
        {
            
            if (!string.IsNullOrEmpty(rolesId))
            {
                //判断用户对应的角色集合是否存在，如果不存在则创建
                if (user.RoleList == null)
                    user.RoleList = new List<Role>();

                //字符串修改（替换）
                string strArray = rolesId.Replace("\r\n                    ", "");
                string strArray2 = strArray.Replace("\r\n                ", "");
                //将id值拆分到数组里面
                string[] ids = strArray2.Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    //判断是否为空
                    if (string.IsNullOrEmpty(id))
                        continue;

                    //向用户角色集合中添加角色数据
                    user.RoleList.Add(new RoleComponent().Get(int.Parse(id)));
                }
            }

            //保存用户，同时将用户所对应的角色全部保存到数据库中
            manager.Create(user);
        }


        /// <summary>
        /// 帐号重复性检测
        /// </summary>
        /// <param name="id">帐号对应的ID值</param>
        /// <param name="account">要判断的帐号名</param>
        /// <returns></returns>
        public bool AccountCheck(int? id, string account)
        {
            //组织参数条件
            IList<ICriterion> criterionList = new List<ICriterion>();
            criterionList.Add(Expression.Eq("Account", account));
            //判断id是否值大于0，如果是则说明是修改时做判断
            if (id.HasValue && id.Value > 0)
            {
                //criterionList.Add(Expression.Not(Expression.Eq("ID", id.Value)));
                ICriterion criterion = Expression.Eq("ID", id.Value);
                criterionList.Add(Expression.Not(criterion));
            }

            //执行查询操作
            return this.Exists(criterionList);

        }


        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="id"></param>
        public new void Delete(int id)
        {
            manager.Delete(id);
        }

        /// <summary>
        /// 通过对象删除
        /// </summary>
        /// <param name="user"></param>
        public void Delete(User user)
        {
            manager.Delete(user);
        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">重置对象id</param>
        public void ResetPasswords(int id)
        {
            //根据id获取实体
            User newUser = manager.Get(id);
            //重置密码为（123456）
            newUser.Password = "E10ADC3949BA59ABBE56E057F20F883E";
            manager.Update(newUser);
        }
    }
}
