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
    public class EasyShoppingCentreController : BasesController
    {
     


        #region 商城首页
        public ActionResult YiGouShangCheng(string goodsName)
        {
            //准备查询条件
            List<ICriterion> newGoods = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(goodsName))
            {
                newGoods.Add(Expression.Like("GoodsName","%"+ goodsName+"%"));
            }

            IList<Goods> list = Container.Instance.Resolve<IGoodsService>().GetAll(newGoods).Where(o=>o.IsActive==true).ToList();

            return View(list);
            
        }
        #endregion


        #region 商品详情页
        [HttpGet]
        public ActionResult Goods(string id="")
        {
            if (id == "" || id == null)
            { return RedirectToAction("YiGouShangCheng"); }//跳转到列表视图}
            Goods goods = Container.Instance.Resolve<IGoodsService>().Get(int.Parse(id));
            return View(goods);
        }
        #endregion


        #region 购物车
        public ActionResult Shoppingcart()
        {

            List<ICriterion> users = new List<ICriterion>();
            users.Add(Expression.Eq("CustomerId.ID", AppHelper.LoginedCustomer.ID));

            IList<ShoppingCart> list = Container.Instance.Resolve<IShoppingCartService>().GetAll(users);
            return View(list);
        }
        #endregion


        #region 删除购物车商品
        public ActionResult Delete(int id)
        {
            Container.Instance.Resolve<IShoppingCartService>().Delete(id);
            return RedirectToAction("Shoppingcart");//跳转到列表视图
        }
        #endregion


        #region 加入购物车
        public ActionResult Cart(int ids,int Quantity)
        {
            ShoppingCart cart = new ShoppingCart();
            if (AppHelper.LoginedCustomer == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");//跳转页面
            }
            cart.CustomerId = AppHelper.LoginedCustomer;
            cart.Quantity = Quantity;
            cart.GoodsId = Container.Instance.Resolve<IGoodsService>().Get(ids);
            Container.Instance.Resolve<IShoppingCartService>().Create(cart);
            return RedirectToAction("Goods", new { id = ids });
        }
        #endregion


        #region 已买到的宝贝
        public ActionResult Salesorder()
        {

            List<ICriterion> users = new List<ICriterion>();
            users.Add(Expression.Eq("CustomerId.ID", AppHelper.LoginedCustomer.ID));

            IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().GetAll(users);
            return View(list);
        }
        #endregion


        #region 添加客户
        [HttpGet]
        public ActionResult Create()
        {
            Customer customer = new Customer();//实体化一个空的实体
            return View( customer);
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            customer.Integral = 0;//初始化积分
            customer.VIP = false;
            customer.IsActive = true;

            //如果未上传图片
            if (AppHelper.Picture == "" || AppHelper.Picture == null)
            {
                return View(customer);
            }
            customer.Picture = AppHelper.Picture;

            ModelState.Remove("VIP");
            ModelState.Remove("IsActive");
            ModelState.Remove("Picture");
            ModelState.Remove("Integral");
            ModelState.Remove("Password");//模型验证时除去密码
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
                //判断是否存在
                List<ICriterion> queryCondition = new List<ICriterion>();
                queryCondition.Add(Expression.Eq("Account", customer.Account));
                if (Container.Instance.Resolve<ICustomerService>().Exists(queryCondition))
                {
                    ModelState.AddModelError("Account", "账号已存在");//返回提示信息
                    return View("Create", customer);
                }

                customer.Password = AppHelper.EncodeMd5(customer.Password);
                Container.Instance.Resolve<ICustomerService>().Create(customer);
                return RedirectToAction("YiGouShangCheng");
            }
            return View(customer);
        }
        #endregion


        #region 取消订单
        public ActionResult Deletes(int id)
        {
            SalesOrder news = Container.Instance.Resolve<ISalesOrderService>().Get(id);

            //库存修改
            foreach(var item in news.SalesOrderDetails)
            {
                Goods newg = Container.Instance.Resolve<IGoodsService>().Get(item.GoodsId.ID);
                newg.Stock += news.Quantity;
                Container.Instance.Resolve<IGoodsService>().Update(newg);
            }

            Container.Instance.Resolve<ISalesOrderService>().Delete(id);
            return RedirectToAction("Salesorder");//跳转到列表视图
        }
        #endregion


        #region 确认收货
        public ActionResult Detailss(int id)
        {
            //修改订单状态
            SalesOrder news= Container.Instance.Resolve<ISalesOrderService>().Get(id);
            news.Status = "完成";
            Container.Instance.Resolve<ISalesOrderService>().Update(news);

            return RedirectToAction("Salesorder");//跳转到列表视图
        }
        #endregion


        #region 详情
        public ActionResult Xq(int id)
        {
            return View(Container.Instance.Resolve<ISalesOrderService>().Get(id).SalesOrderDetails);
        }
        #endregion


        #region 修改基本信息(登录用户)
        [HttpGet]
        public ActionResult EditloginUser()
        {

            Customer customer = Container.Instance.Resolve<ICustomerService>().Get(AppHelper.LoginedCustomer.ID);//根据id获取Customer实体
            return View(customer);//将这个user实体显示在edit页面上对应的控件
        }

        [HttpPost]
        public ActionResult EditloginUser(Customer model)
        {
            Customer customer = Container.Instance.Resolve<ICustomerService>().Get(model.ID);

            //判断是否存在
            if (Container.Instance.Resolve<IUserService>().AccountCheck(model.ID, model.Account))
            {
                ModelState.AddModelError("Account", "帐号已存在");//返回提示信息
                return View(customer);//停留在原页面,返回User对象，目的是保留提交前输入的用户信息
            }
            else
            {
                customer.Account = model.Account;
            }

            ModelState.Remove("VIP"); 
            ModelState.Remove("IsActive");
            ModelState.Remove("Picture");
            ModelState.Remove("Integral");
            ModelState.Remove("Password");//模型验证时除去密码

            if (ModelState.IsValid)
            {
                customer.Number = model.Number;
                customer.Sex = model.Sex;
                customer.CustomerName = model.CustomerName; customer.Email = model.Email; customer.Birthday = model.Birthday; customer.Account = model.Account;
                Container.Instance.Resolve<ICustomerService>().Update(customer);
                return View(customer);
            }
            return View(customer);
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
            if (AppHelper.EncodeMd5(oldPassword) == AppHelper.LoginedCustomer.Password)
            {
                if (AppHelper.EncodeMd5(oldPassword) == AppHelper.LoginedCustomer.Password && newPassword == newPasswords)
                {
                    Customer newUser = Container.Instance.Resolve<ICustomerService>().Get(AppHelper.LoginedCustomer.ID);
                    newUser.Password = AppHelper.EncodeMd5(newPassword);
                    Container.Instance.Resolve<ICustomerService>().Update(newUser);
                    return "密码修改成功！！！";
                }
                else { return "密码修改失败！！！"; }
            }
            else { return "原始密码输入错误！！！"; }


        }
        #endregion


        #region 地址管理（查看、新建）
        [HttpGet]
        public ActionResult EditAddress()
        {
            //获取当前登录客户所有地址
            IList< Address> address = Container.Instance.Resolve<IAddressService>().GetAll().Where(o=>o.CustomerId.ID==AppHelper.LoginedCustomer.ID).ToList();

            return View(address);
        }

        [HttpPost]
        public ActionResult EditAddress(Address model)
        {
            //获取当前登录客户所有地址
            IList<Address> addresss = Container.Instance.Resolve<IAddressService>().GetAll().Where(o => o.CustomerId.ID == AppHelper.LoginedCustomer.ID).ToList();

            ModelState.Remove("CustomerId");//模型验证时除去
             if (ModelState.IsValid)
             {
                 model.CustomerId = AppHelper.LoginedCustomer;
                 //第一条地址默认为默认地址
                 if (addresss.Count == 0)
                 {
                     model.IsActive = true;
                 }
                 Container.Instance.Resolve<IAddressService>().Create(model);
             }

             //获取当前登录客户(新增)所有地址
             IList<Address> address = Container.Instance.Resolve<IAddressService>().GetAll().Where(o => o.CustomerId.ID == AppHelper.LoginedCustomer.ID).ToList();

            return View(address);
        }
        #endregion


        #region 删除地址
        public ActionResult DeleteAddress(int id)
        {
            Container.Instance.Resolve<IAddressService>().Delete(id);
            return RedirectToAction("EditAddress");//跳转到列表视图
        }
        #endregion


        #region 修改默认地址
        public ActionResult EditIsActive(int id)
        {
            //修改原来默认地址
            IList< Address> addresss = Container.Instance.Resolve<IAddressService>().GetAll().Where(o=>o.IsActive==true).ToList();
            foreach (var item in addresss)
            {
                item.IsActive = false;
                Container.Instance.Resolve<IAddressService>().Update(item);
            }

            //修改当前地址
            Address address= Container.Instance.Resolve<IAddressService>().Get(id);
            address.IsActive = true;
            Container.Instance.Resolve<IAddressService>().Update(address);

            return RedirectToAction("EditAddress");//跳转到列表视图
        }
        #endregion


        #region 购买商品页面
        [HttpGet]
        public ActionResult Purchase(int id, int Quantity, string car = "")//商品id，购买数量
        {
            Goods goods = Container.Instance.Resolve<IGoodsService>().Get(id);
            ViewBag.Quantity = Quantity;
            ViewBag.Car = car;

            ViewBag.Address = Container.Instance.Resolve<IAddressService>().GetAll().Where(o => o.CustomerId.ID == AppHelper.LoginedCustomer.ID).ToList();

            return View(goods);
            
        }

        [HttpPost]
        public ActionResult Purchase(int ids, int Quantity,string dz="",string car="")//商品id、购买数量、地址id
        {
            //若果为购物车购物则删除购物车
            if (car != "" && car != null)
            { Container.Instance.Resolve<IShoppingCartService>().Delete(int.Parse(car)); }
            //如果无地址转跳到添加地址页面
            if (dz == "" || dz == null)
            { return RedirectToAction("EditAddress");}

            Goods goods = Container.Instance.Resolve<IGoodsService>().Get(ids);

            if (goods.Stock >= Quantity&&Quantity>0)
            {
                //实例化订单
                SalesOrder SalesOrder = new SalesOrder();
                SalesOrder.SalesOrderDetails = new List<SalesOrderDetails>() { new SalesOrderDetails() };

                //销售单赋值
                SalesOrder.CustomerId = AppHelper.LoginedCustomer;
                SalesOrder.Money = goods.OutPrice * Quantity;
                SalesOrder.Quantity = Quantity;
                SalesOrder.Remarks = "购买";
                SalesOrder.Status = "已付款";
                SalesOrder.Addressee = Container.Instance.Resolve<IAddressService>().Get(int.Parse(dz)).Addressee;
                SalesOrder.Addressing = Container.Instance.Resolve<IAddressService>().Get(int.Parse(dz)).Addressing;
                SalesOrder.Time = DateTime.Now;
                SalesOrder.Profits = (goods.OutPrice - goods.InnPrice) * Quantity;//利润

                //库存修改
                goods.Stock -= Quantity;
                Container.Instance.Resolve<IGoodsService>().Update(goods);

                //销售明细
                foreach (var item in SalesOrder.SalesOrderDetails)
                {
                    item.Quantity = Quantity;
                    item.GoodsId = goods;
                    item.SalesOrder = SalesOrder;

                }

                Container.Instance.Resolve<ISalesOrderService>().Create(SalesOrder);
            }
            else { return RedirectToAction("Purchase", new { id = ids,Quantity=Quantity,car=car }); }

            return RedirectToAction("Salesorder");//跳转页面

            
        }
        #endregion


        #region 退货
        [HttpPost]
        public string Return(int id)
        {
            SalesOrder sales = Container.Instance.Resolve<ISalesOrderService>().Get(id);

            //15天无理由退货
            if (DateTime.Now.Subtract(sales.Time).Days <= 15)
            {
                sales.Status = "寄回中";
                sales.Remarks = "退货";
                Container.Instance.Resolve<ISalesOrderService>().Update(sales);
                return "操作成功！！！";
            }
               
            return "超过15天已经无法退货！！！";//跳转到列表视图
        }
        #endregion


        #region 付款
        public ActionResult Pay()
        {

            return View();
        }
        #endregion


    }
}