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
    public class RoleComponent:BaseComponent<Role,RoleManager>,IRoleService
    {
        /// <summary>
        /// 菜单配置
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="rolesId"></param>
        public void AssignRole(int userId, string rolesId)
        {
            //查询用户
            Role user = manager.Get(userId);

            //判断用户对应的角色集合是否存在，如果不存在则创建
            if (user.SysFunctionList == null)
                user.SysFunctionList = new List<SystemFunction>();

            //清空所有的角色
            user.SysFunctionList.Clear();

            if (!string.IsNullOrEmpty(rolesId))
            {
                string[] ids = rolesId.Split(new char[] { ',', '_', '|' });//将id值拆分到数组里面
                foreach (string tempId in ids)
                {
                    //判断是否为空
                    if (string.IsNullOrEmpty(tempId))
                        continue;

                    //增加角色到集合中
                    user.SysFunctionList.Add(new SystemFunctionManager().Get(int.Parse(tempId)));
                }
            }

            //修改数据
            manager.Update(user);
        }

        /// <summary>
        /// 角色修改
        /// </summary>
        /// <param name="model">修改后的角色对象</param>
        public bool RoleEdit(Role model)
        {
            //准备查询条件
            List<ICriterion> newrole = new List<ICriterion>();
            //根据角色名查询（唯一性）
            newrole.Add(Expression.Eq("RoleName", model.RoleName));
            //根据id查询角色
            Role newRole = manager.Get(model.ID);
            if (newRole.RoleName==model.RoleName||!manager.Exists(newrole))
            {
                newRole.RoleName = model.RoleName;
                newRole.IsActive = model.IsActive;
                manager.Update(newRole);
                //跳转到Index页面
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 注销激活
        /// </summary>
        /// <param name="id">注销激活对象id</param>
        public void SwitchStatus(int id)
        {
            //根据id获取对象实体
            Role newRole = manager.Get(id);
            //判断当前对象状态并修改
            if (newRole.IsActive)
            {
                newRole.IsActive = false;
            }
            else
            {
                newRole.IsActive = true;
            }
            //传入数据库
            manager.Update(newRole);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <returns></returns>
        public bool Createrole(Role role)
        {
            List<ICriterion> newrole = new List<ICriterion>();
            newrole.Add(Expression.Eq("RoleName", role.RoleName));

            if (manager.Exists(newrole))
            {
                return true;
            }
            else
            {
                manager.Create(role);
                return false;
            }
        }
    }
}
