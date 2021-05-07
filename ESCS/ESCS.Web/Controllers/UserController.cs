using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESCS.Web.Apps;
using ESCS.Domain;
using ESCS.Service;
using ESCS.Core;
using NHibernate.Criterion;
using System.IO;

namespace ESCS.Web.Controllers
{
    public class UserController : BaseController
    {
        #region 登录
        [HttpGet]
        public ActionResult Login()
        {
            //清除以前登录用户信息
            AppHelper.LoginedUser = null;
            return View();
        }

        [HttpPost]
        public string Login(string account, string pwd)
        {
            pwd = AppHelper.EncodeMd5(pwd);
            //把获取的密码和账号发给Login
            User loginedUser = Container.Instance.Resolve<IUserService>().Login(account, pwd);
            if (loginedUser != null)
            {
                //如果用户存在保存登录用户信息
                AppHelper.LoginedUser = loginedUser;
                return "1";
            }
            else
            { return "密码或账号错误"; }
        }
        #endregion


        #region 用户集合
        public ActionResult Index(string userName = "")
        {
            //准备查询条件
            List<ICriterion> newuser = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(userName))
            {
                newuser.Add(Expression.Or(
                     Expression.Eq("UserName", userName),
                     Expression.Eq("Account", userName)
                     ));
            }
            IList<User> lists = Container.Instance.Resolve<IUserService>().GetAll(newuser);
            return View(lists);
        }
        #endregion


        #region 添加用户
        [HttpGet]
        public ActionResult Create()
        {
            User user = new Domain.User();//实体化一个空的实体

            SetRoles();//准备视图需要的角色信息

            return View(user);
        }

        [HttpPost]
        public ActionResult Create(User user, string Roleid)
        {
            user.Password = "E10ADC3949BA59ABBE56E057F20F883E";//系统默认的用户密码‘123456’
            user.IsActive = true;
            //如果未上传图片
            if (AppHelper.Picture == "" || AppHelper.Picture == null)
            {
                SetRoles();//准备视图需要的角色信息
                return View(user);
            }
            user.Picture = AppHelper.Picture;

                //判断是否存在
                if (Container.Instance.Resolve<IUserService>().AccountCheck(0, user.Account))
                {
                    ModelState.AddModelError("Account", "帐号已存在");//返回提示信息
                    SetRoles();//提供角色供视图显示
                    return View(user);//停留在原页面,返回User对象，目的是保留提交前输入的用户信息
                }

                ModelState.Remove("Password"); //模型验证时除去密码
                ModelState.Remove("IsActive");
                ModelState.Remove("Picture");

                if (!string.IsNullOrEmpty(Roleid) && ModelState.IsValid)
                {
                    Container.Instance.Resolve<IUserService>().Create(user, Roleid);
                    return RedirectToAction("Index");//跳转到列表视图
                }
                SetRoles();//提供角色供视图显示
                return View(user);
        }

        private void SetRoles()
        {
            //取出系统中的所有角色
            IList<Role> roleList = Container.Instance.Resolve<IRoleService>().GetAll()
                .Where(m => m.IsActive == true) //使用lamdba表达式过滤未启用状态的角色
                .ToList();  //在使用了lamdba表达式之后要使用ToList()转换成原始对象
            //通过ViewBag传递到视图中去
            ViewBag.RoleList = roleList;
        }
        #endregion


        #region 注销激活
        public ActionResult SwitchStatus(int id)
        {
            Container.Instance.Resolve<IUserService>().SwitchStatus(id);//调用业务逻辑层方法，切换用户状态
            ViewBag.Ts = 1;//提示判断条件
            return RedirectToAction("Index");//跳转到列表页面
        }
        #endregion


        #region 删除用户
        public ActionResult Delete(int id)
        {
            Container.Instance.Resolve<IUserService>().Delete(id);
            return RedirectToAction("Index");//跳转到列表视图
        }
        #endregion


        #region 重置密码
        [HttpGet]
        public string ResetPassword(string userName)
        {
            //准备查询条件
            List<ICriterion> newuser = new List<ICriterion>();
            newuser.Add(Expression.Eq("UserName", userName));
            int id=Container.Instance.Resolve<IUserService>().Get(newuser).ID;
            Container.Instance.Resolve<IUserService>().ResetPasswords(id);//调用事物逻辑层重置密码
            return "重置成功！！！";//跳转到列表视图
        }
        #endregion


        #region 信息查看(修改)

        [HttpGet]
        public ActionResult Details(int id)
        {
            User user = Container.Instance.Resolve<IUserService>().Get(id);//根据id获取user实体

            Roles();

            return View(user);
        }
        [HttpPost]
        public ActionResult Details(User user)
        {
            
            //查询用户
            User users = Container.Instance.Resolve<IUserService>().Get(user.ID);
            //修改工资
            users.Salary = user.Salary;

            Container.Instance.Resolve<IUserService>().Update(users);
            
            return RedirectToAction("Index");//跳转到列表视图
        }

        [HttpGet]
        public ActionResult Detailss(int id)
        {
            Roles();
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public ActionResult Detailss(int id, string Roleid)
        {
            //提供角色
            Roles();
            //查询用户
            User users = Container.Instance.Resolve<IUserService>().Get(id);
            //判断用户对应的角色集合是否存在，如果不存在则创建
            if (users.RoleList == null)
                users.RoleList = new List<Role>();

            if (!string.IsNullOrEmpty(Roleid))
            {
                //清空所有的角色
                users.RoleList.Clear();
                //将id值拆分到数组里面
                string[] ids = Roleid.Split(new char[] { ',' });
                foreach (string tempId in ids)
                {
                    //判断是否为空
                    if (string.IsNullOrEmpty(tempId))
                        continue;
                    //增加角色到集合中
                    users.RoleList.Add(Container.Instance.Resolve<IRoleService>().Get(int.Parse(tempId)));
                }
                Container.Instance.Resolve<IUserService>().Update(users);
                return RedirectToAction("Index");//跳转到列表视图
            }

            return View(users);//跳转到列表视图
        }

        public void Roles()
        {
            //获取所有的并且是启用状态的角色
            //说明：执行本操作前，请先在domain层定义扩展属性IsChecked，用于视图状态的显示
            IList<Role> roleList = Container.Instance.Resolve<IRoleService>().GetAll().Where(m => m.IsActive == true).ToList();

            //判断列表中的角色是否是用户对应的角色，如果是则，则设置当前角色为选中状态
            //if (user.RoleList != null)
            //{
            //    foreach (var item in roleList)
            //    {
            //        if (user.RoleList.Where(m => m.ID == item.ID).Count() > 0)
            //            item.IsChecked = true;
            //    }
            //}

            //通过向视图传递所有的角色信息
            ViewBag.RoleList = roleList;
        }
        #endregion


        #region 修改密码
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public string ChangePassword(string oldPassword, string newPassword, string newPasswords)
        {
            //ChangePasswords(oldPassword,newPassword,newPasswords);
            if (AppHelper.EncodeMd5(oldPassword) == AppHelper.LoginedUser.Password)
            {
                if (AppHelper.EncodeMd5(oldPassword) == AppHelper.LoginedUser.Password && newPassword == newPasswords)
                {
                    User newUser = Container.Instance.Resolve<IUserService>().Get(AppHelper.LoginedUser.ID);
                    newUser.Password = AppHelper.EncodeMd5(newPassword);
                    Container.Instance.Resolve<IUserService>().Update(newUser);
                    return "密码修改成功！！！";
                }
                else { return "密码修改失败！！！"; }
            }
            else { return "原始密码输入错误！！！"; }


        }
        #endregion


        #region 修改基本信息(登录用户)
        [HttpGet]
        public ActionResult EditloginUser()
        {
            User user = Container.Instance.Resolve<IUserService>().Get(AppHelper.LoginedUser.ID);//根据id获取user实体

            return View(user);//将这个user实体显示在edit页面上对应的控件
        }

        [HttpPost]
        public ActionResult EditloginUser(User model)
        {
            ModelState.Remove("RoleList");
            if (ModelState.IsValid)
            {
                //判断是否存在
                if (Container.Instance.Resolve<IUserService>().AccountCheck(model.ID, model.Account))
                {
                    ModelState.AddModelError("Account", "帐号已存在");//返回提示信息
                    return View(model);//停留在原页面,返回User对象，目的是保留提交前输入的用户信息
                }
                model.RoleList = Container.Instance.Resolve<IUserService>().Get(AppHelper.LoginedUser.ID).RoleList;
                model.IsActive = true;
                Container.Instance.Resolve<IUserService>().Update(model);
                return View(model);
 
            }
            return View(model);
        }
        #endregion


        #region 工资管理
        [HttpGet]
        public ActionResult Salary(int pageIndex = 1)
        {
            IList<User> lists = Container.Instance.Resolve<IUserService>().GetAll();
            return View(lists);
        }
        [HttpPost]
        public string Salary(DateTime time)
        {
            //判断数据库有无数据
            if (Container.Instance.Resolve<IBillService>().GetAll().Count() == 0)
            {
                Bill lastBill = new Bill();

                //保存账单
                if (xx(lastBill, time))
                {
                    return "保存成功";
                }
                return "该月已结算工资";
            }
            else
            {
                //获取最后一条账单记录
                Bill lastBill = Container.Instance.Resolve<IBillService>().GetAll().Last();
                lastBill.Money = 0;

                //保存账单
                if (xx(lastBill, time))
                {
                    return "工资结算成功！！！";
                }
                return "该月已结算工资！！！";
            }
        }
        public bool xx(Bill lastBill,DateTime time)
        {

            IList<Bill> list = Container.Instance.Resolve<IBillService>().GetAll().Where(o => o.Remarks == time.Year + "年" + time.Month + "月全体工资").ToList() ;
            if (list.Count == 0)
            {
                IList<User> userlist = Container.Instance.Resolve<IUserService>().GetAll();

                //循环累加所有员工工资
                foreach (var item in userlist)
                {
                    lastBill.Money += item.Salary;
                }

                lastBill.Remarks = time.Year + "年" + time.Month + "月全体工资";
                lastBill.Balance -= lastBill.Money;
                lastBill.UserId = AppHelper.LoginedUser;
                lastBill.Time = DateTime.Now;
                //创建新账单
                Container.Instance.Resolve<IBillService>().Create(lastBill);

                return true;
            }
            return false;
           
        }
        #endregion


        #region 业绩
        public ActionResult Performance(int id)
        {
            User model= Container.Instance.Resolve<IUserService>().Get(id);


            //准备查询条件
            List<ICriterion> newuser = new List<ICriterion>();
            newuser.Add(Expression.Eq("UserId.ID", id));

            IList<SalesOrder> sales = Container.Instance.Resolve<ISalesOrderService>().GetAll(newuser).Where(item => item.Status == "货已送到").ToList();

            //准备查询条件
            List<ICriterion> newusers = new List<ICriterion>();
            newusers.Add(Expression.Eq("UserIds.ID", id));


            IList<PurchaseOrder> purchaseOrder = Container.Instance.Resolve<IPurchaseOrderService>().GetAll(newusers).Where(item => item.Status == "结束").ToList();
            ViewBag.PurchaseOrder = purchaseOrder;

            return View(sales);
        }
        #endregion

    }
   
}