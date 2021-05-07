using ESCS.Core;
using ESCS.Domain;
using ESCS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESCS.Web.Apps;
using NHibernate.Criterion;

namespace ESCS.Web.Controllers
{
    public class RoleController : BaseController
    {
       
        #region 角色管理
        public ActionResult Index(string roleName)
        {
            //准备查询条件
            List<ICriterion> newrole = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(roleName))
            {
                newrole.Add(Expression.Eq("RoleName", roleName) );
            }

            IList<Role> roleList = Container.Instance.Resolve<IRoleService>().GetAll(newrole);

            //返回到Index的强类型视图
            return View(roleList);
        }
        #endregion


        #region 角色创建

        [HttpGet]
        public ActionResult Create()
        {
            As();
            Role role = new Role();
            return View(role);
        }

        [HttpPost]
        public ActionResult Create(Role role, string Roleid)
        {
            As();
            role.IsActive = true;
            ModelState.Remove("IsActive"); //不验证状态
            if (ModelState.IsValid)//如果role实体的数据合法
            {
                if (!string.IsNullOrEmpty(Roleid))
                {
                    //判断用户对应的角色集合是否存在，如果不存在则创建
                    if (role.SysFunctionList == null)
                        role.SysFunctionList = new List<SystemFunction>();


                    string[] ids = Roleid.Split(new char[] { ',' });
                    List<string> list = new List<string>();
                    for (int i = 0; i < ids.Length; i++)//遍历数组成员
                    {
                        if (list.IndexOf(ids[i].ToLower()) == -1)//对每个成员做一次新数组查询如果没有相等的则加到新数组
                            list.Add(ids[i]);

                    }
                    foreach (string id in list)
                    {
                        //判断是否为空
                        if (string.IsNullOrEmpty(id))
                            continue;

                        //向用户角色集合中添加角色数据
                        role.SysFunctionList.Add(Container.Instance.Resolve<ISystemFunctionService>().Get(int.Parse(id)));
                    }
                    Container.Instance.Resolve<IRoleService>().Create(role);
                    return RedirectToAction("Index");
                }
            }
            return View(role);
        }

        public void As()
        {
        //获取所有菜单信息
            IList<SystemFunction> FunctionList = Container.Instance.Resolve<ISystemFunctionService>().GetAll().Where(m => m.IsShow == true).ToList();
            //if (model.SysFunctionList != null)
            //{
            //    foreach (var item in FunctionList)
            //    {
            //        if (model.SysFunctionList.Where(m => m.ID == item.ID).Count() > 0)
            //            item.IsShow = true;
            //    }
            //}
            //菜单信息传入视图
            ViewBag.List = FunctionList;
        }
        #endregion


        #region 角色修改
        [HttpGet]
        public string Edit(int id)
        {
            //根据id获取实体
            Role model = Container.Instance.Resolve<IRoleService>().Get(id);
            return model.RoleName;
        }
        [HttpPost]
        public string Edit(string name, int id)
        {
            Role newKind = Container.Instance.Resolve<IRoleService>().Get(id);

            List<ICriterion> kindName = new List<ICriterion>();
            kindName.Add(Expression.Eq("RoleName", name));

            //如修改判断是否存在再修改，若果未修改则保存原来信息
            if (Container.Instance.Resolve<IRoleService>().Get(id).RoleName == name || !Container.Instance.Resolve<IRoleService>().Exists(kindName))
            {
                newKind.RoleName = name;
                Container.Instance.Resolve<IRoleService>().Update(newKind);
                return "修改成功！！！";

            }
            else
            {
                return "名字已存在！！！";
            }
        }
        #endregion


        #region 删除
        public ActionResult Delete(int id)
        {
            //根据角色ID查询角色信息
            Role model = Container.Instance.Resolve<IRoleService>().Get(id);

            //判断角色是否已分配了用户，如果有，则不能删除
            if (model.UserList == null || model.UserList.Count == 0)
                Container.Instance.Resolve<IRoleService>().Delete(model);
            else
            {
                //返回错误提示
            }
            //跳转到Index页面
            return RedirectToAction("Index");
        }
        #endregion    


        #region 注销激活
        public ActionResult SwitchStatus(int id)
        {
            
            Container.Instance.Resolve<IRoleService>().SwitchStatus(id);//调用业务逻辑层方法，切换用户状态

            return RedirectToAction("Index");//跳转到列表页面
        }
        #endregion


        #region 菜单权限配置
        [HttpGet]
        public ActionResult Authorize(int id)
        {
            //根据id获取角色
            Role model = Container.Instance.Resolve<IRoleService>().Get(id);
            //获取所有菜单信息
            As();
            return View(model);
        }

        [HttpPost]
        public ActionResult Authorize(Role role, string Roleid)
        {
            Role newRole= Container.Instance.Resolve<IRoleService>().Get(role.ID);
            newRole.SysFunctionList.Clear();//清空以前项
            As();
            if (!string.IsNullOrEmpty(Roleid))
            {
                //判断用户对应的角色集合是否存在，如果不存在则创建
                if (newRole.SysFunctionList == null)
                    newRole.SysFunctionList = new List<SystemFunction>();


                //将id值拆分到数组里面
                string[] ids = Roleid.Split(new char[] { ',' });
                List<string> list = new List<string>();
                for (int i = 0; i < ids.Length; i++)//遍历数组成员
                {
                    if (list.IndexOf(ids[i].ToLower()) == -1)//对每个成员做一次新数组查询如果没有相等的则加到新数组
                        list.Add(ids[i]);

                }
                foreach (string id in list)
                {
                    //判断是否为空
                    if (string.IsNullOrEmpty(id))
                        continue;

                    //向用户角色集合中添加角色数据
                    newRole.SysFunctionList.Add(Container.Instance.Resolve<ISystemFunctionService>().Get(int.Parse(id)));
                }
                Container.Instance.Resolve<IRoleService>().Update(newRole);
                return RedirectToAction("Index");
            }
            return View(newRole);
        }
        #endregion
    }
}
