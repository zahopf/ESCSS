using ESCS.Core;
using ESCS.Domain;
using ESCS.Service;
using ESCS.Web.Apps;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESCS.Web.Controllers
{
    public class CustomerController : BaseController
    {
        #region 登录
        [HttpGet]
        public ActionResult CustomerLogin()
        {
            AppHelper.LoginedCustomer = null;
            return View();
        }

        [HttpPost]
        public string CustomerLogin(string account, string pwd)
        {
            pwd = AppHelper.EncodeMd5(pwd);

            //组织查询条件(标准条件查询)
            IList<NHibernate.Criterion.ICriterion> criterionList = new List<ICriterion>();
            //向条件集合添加查询条件，每个条件之间默认为And关系，从数据库中查询同时满足3个条件的用户信息
            criterionList.Add(Expression.Eq("Account", account));
            criterionList.Add(Expression.Eq("Password", pwd));
            criterionList.Add(Expression.Eq("IsActive", true));

            //调用数据访问层的方法执行操作       
            Customer customer = Container.Instance.Resolve<ICustomerService>().Get(criterionList);

            if (customer != null)
            {
                //如果用户存在保存登录用户信息
                AppHelper.LoginedCustomer= customer;
                return "1";
            }
            else
            { return "密码或账号错误"; }
        }
        #endregion


        #region 客户集合
        public ActionResult Index(string userName = "")
        {
            //准备查询条件
            List<ICriterion> newuser = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(userName))
            {
                newuser.Add(Expression.Or(
                     Expression.Eq("CustomerName", userName),
                     Expression.Eq("Number", userName)
                     ));
            }

            IList<Customer> lists = Container.Instance.Resolve<ICustomerService>().GetAll(newuser);
            return View(lists);
        }
        #endregion


        #region 添加用户
        [HttpGet]
        public ActionResult Create()
        {
            Customer customer = new Customer();//实体化一个空的实体
            return View("Create", customer);
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            customer.Integral = 0;//初始化积分
            if (ModelState.IsValid)
            {
                //判断是否存在
                List<ICriterion> queryConditions = new List<ICriterion>();
                queryConditions.Add(Expression.Eq("Number", customer.Number));
                if (Container.Instance.Resolve<ICustomerService>().Exists(queryConditions))
                {
                    ModelState.AddModelError("Number", "电话号码已存在");//返回提示信息
                    return View("Create", customer);
                }
                else
                {
                    customer.Password = AppHelper.EncodeMd5(customer.Password);
                    Container.Instance.Resolve<ICustomerService>().Create(customer);
                    return RedirectToAction("Index");//跳转到Index页面
                }
            }
            return View(customer);
        }
        #endregion


        #region 信息查看
        [HttpGet]
        public ActionResult Details(int id)
        {
            Customer user = Container.Instance.Resolve<ICustomerService>().Get(id);//根据id获取user实体


            return View(user);
        }
        [HttpPost]
        public ActionResult Details(User user, string Roleid)
        {
           

            return View();//跳转到列表视图
        }
        #endregion


        #region 注销激活
        public ActionResult SwitchStatus(int id)
        {
            Customer user = Container.Instance.Resolve<ICustomerService>().Get(id);//根据id获取user实体
            user.IsActive = !user.IsActive;//设置user的状态为“非”当前状态，即如果当前是活动的则设置为不活动，如果当前是不活动的则设置为活动
            Container.Instance.Resolve<ICustomerService>().Update(user);//更新当前用户
            return RedirectToAction("Index");//跳转到列表页面
        }
        #endregion


        #region VIP注销激活
        public ActionResult VIP(int id)
        {
            Customer user = Container.Instance.Resolve<ICustomerService>().Get(id);//根据id获取user实体
            user.VIP = !user.VIP;//设置user的状态为“非”当前状态，即如果当前是活动的则设置为不活动，如果当前是不活动的则设置为活动
            Container.Instance.Resolve<ICustomerService>().Update(user);//更新当前用户
            return RedirectToAction("Index");//跳转到列表页面
        }
        #endregion


        #region 删除用户
        public ActionResult Delete(int id)
        {
            Container.Instance.Resolve<ICustomerService>().Delete(id);
            return RedirectToAction("Index");//跳转到列表视图
        }
        #endregion


        #region 订单信息
        public ActionResult AllSalesOrder(int id, string remarks = "")
        {
            Customer cs = Container.Instance.Resolve<ICustomerService>().Get(id);

            IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().GetAll().Where(o => o.CustomerId.ID == cs.ID).ToList() ;

            //状态查询
            if (remarks != "" && remarks != null)
            {
                list = list.Where(o => o.Remarks == remarks).ToList();
            }

            return View(list);
        }
        #endregion
	}
}