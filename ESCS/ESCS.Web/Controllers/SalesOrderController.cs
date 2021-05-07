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
    public class SalesOrderController : BaseController
    {
        #region 销售单
        public ActionResult Index()
        {
            IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().GetAll();
            return View(list);
        }
        #endregion


        #region 订单申请
        public ActionResult Application()
        {

            IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().GetAll().Where(m => m.Status == "已付款").ToList();
            return View(list);
        }
        #endregion
     
        #region 订单申请ing
        public ActionResult Applicationing(int id)
        {
            //根据ID查询信息
            SalesOrder model = Container.Instance.Resolve<ISalesOrderService>().Get(id);
            model.UserId = AppHelper.LoginedUser;//修改进货员
            model.Status = "配送中";//修改状态
            Container.Instance.Resolve<ISalesOrderService>().Update(model);

            return RedirectToAction("Application");
        }
        #endregion

        #region 订单完成
        public ActionResult Applicationings(int id)
        {
            //根据ID查询信息
            SalesOrder model = Container.Instance.Resolve<ISalesOrderService>().Get(id);
            model.Status = "货已送到";//修改状态

            //创建账单
            Bill bill = new Bill();

            //获取最后一条账单记录
            Bill lastBill = Container.Instance.Resolve<IBillService>().GetAll().Last();
            //赋值
            bill.Money = model.Profits;
            bill.Remarks = "销售";
            bill.Balance = lastBill.Balance + model.Profits;//销售后余额
            bill.UserId = model.UserId;
            bill.Time = DateTime.Now;
            //创建新账单
            Container.Instance.Resolve<IBillService>().Create(bill);

            Container.Instance.Resolve<ISalesOrderService>().Update(model);

            return RedirectToAction("Own");
        }
        #endregion

        #region 登录用户拥有订单
        public ActionResult Own(string modules)
        {
            //准备查询条件
            List<ICriterion> newPurchaseOrder = new List<ICriterion>();
            newPurchaseOrder.Add(Expression.Eq("UserId.ID", AppHelper.LoginedUser.ID));
            //判断是否为空
            if (!string.IsNullOrEmpty(modules))
            {
                if (modules != "1")
                { newPurchaseOrder.Add(Expression.Eq("Status", modules)); }
            }

            IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().GetAll(newPurchaseOrder);
            return View(list);
        }
        #endregion

        #region 详情
        public ActionResult Xq(int id)
        {

            return View(Container.Instance.Resolve<ISalesOrderService>().Get(id).SalesOrderDetails);
        }
        #endregion 

        #region 确认收货
        public ActionResult Confirmed(int id)
        {
            //根据ID查询信息
            SalesOrder model = Container.Instance.Resolve<ISalesOrderService>().Get(id);
            model.Status = "完成";//修改状态

            //返回库存
            foreach (var item in model.SalesOrderDetails)
            {
                Goods good = Container.Instance.Resolve<IGoodsService>().Get(item.GoodsId.ID);
                good.Stock += item.Quantity;
                Container.Instance.Resolve<IGoodsService>().Update(good);
            }
            

            //创建账单
            Bill bill = new Bill();

            //获取最后一条账单记录
            Bill lastBill = Container.Instance.Resolve<IBillService>().GetAll().Last();
            //赋值
            bill.Money = model.Profits;
            bill.Remarks = "退货";
            bill.Balance = lastBill.Balance - model.Profits;//销售后余额
            bill.UserId = model.UserId;
            bill.Time = DateTime.Now;
            //创建新账单
            Container.Instance.Resolve<IBillService>().Create(bill);

            Container.Instance.Resolve<ISalesOrderService>().Update(model);

            return RedirectToAction("Own");
        }
        #endregion 

	}
}