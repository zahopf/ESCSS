using ESCS.Domain;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Service
{
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">操作员输入的用户名</param>
        /// <param name="password">操作员输入的密码</param>
        /// <returns>当用户名和密码成功匹配时返回匹配的用户信息，否则返回null</returns>
        User Login(string account, string password);

        /// <summary>
        /// 重写Create
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hdSelectedIds"></param>
        void Create(User user, string hdSelectedIds);

        /// <summary>
        /// 根据id修改实体状态
        /// </summary>
        /// <param name="id">要修改对象的id</param>
        void SwitchStatus(int id);


        /// <summary>
        /// 帐号重复性检测
        /// </summary>
        /// <param name="id">帐号对应的ID值</param>
        /// <param name="account">要判断的帐号名</param>
        /// <returns></returns>
        bool AccountCheck(int? id, string account);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">重置对象id</param>
        void ResetPasswords(int id);

    }
}
