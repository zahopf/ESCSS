using ESCS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.Service
{
     public interface IRoleService:IBaseService<Role>
    {
         void AssignRole(int userId, string rolesId);

         /// <summary>
         /// 角色修改
         /// </summary>
         /// <param name="model">修改后的角色对象</param>
         bool RoleEdit(Role model);

         /// <summary>
         /// 注销激活
         /// </summary>
         /// <param name="id">注销激活对象id</param>
         void SwitchStatus(int id);

         /// <summary>
         /// 创建角色
         /// </summary>
         /// <param name="role">角色对象</param>
         /// <returns></returns>
         bool Createrole(Role role);
    }
}
