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
    public class PurchaseOrderController : BaseController
    {
        #region 订单显示
        public ActionResult Index(string modules)
        {
            //准备查询条件
            List<ICriterion> newPurchaseOrder = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(modules))
            {
                if (modules != "1")
                { newPurchaseOrder.Add(Expression.Eq("Status", modules)); }
            }

            IList<PurchaseOrder> list = Container.Instance.Resolve<IPurchaseOrderService>().GetAll(newPurchaseOrder);
            return View(list);
        }
        #endregion


        #region 进货单创建
        [HttpGet]
        public ActionResult Create()
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetails = new List<PurchaseOrderDetails>() { new PurchaseOrderDetails() };

            return View(purchaseOrder);
        }

        [HttpPost]
        public ActionResult Create(PurchaseOrder model,int pd)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.PurchaseOrderDetails = new List<PurchaseOrderDetails>() { new PurchaseOrderDetails() };

            //搜索
            if (pd == 0)
            {
                List<ICriterion> goodsName = new List<ICriterion>();
                goodsName.Add(Expression.Eq("GoodsName", Request["GoodsName"]));
                Goods goods = Container.Instance.Resolve<IGoodsService>().Get(goodsName);
                if (goods != null)
                {
                    ViewBag.GoodsName = goods.GoodsName;
                    ViewBag.InnPrice = goods.InnPrice;
                    ViewBag.OutPrice = goods.OutPrice;
                    ViewBag.Stock = goods.Stock;
                    ViewBag.Id = goods.ID;
                }
                if (model.ID != 0)
                { purchaseOrder = Container.Instance.Resolve<IPurchaseOrderService>().Get(model.ID); }
                
                return View(purchaseOrder);
            }
            //添加
            if (pd == 1)
            {
                
                if (model.ID == 0)
                {
                    //创建经货单基本数据
                    purchaseOrder.Time = DateTime.Now;
                    purchaseOrder.Status = "待分配";
                    purchaseOrder.UserId = AppHelper.LoginedUser;
                    Container.Instance.Resolve<IPurchaseOrderService>().Create(purchaseOrder);
                }
                else
                {purchaseOrder = Container.Instance.Resolve<IPurchaseOrderService>().Get(model.ID); }


                //明细添加
                PurchaseOrderDetails newp = new PurchaseOrderDetails();
                newp.GoodsId = Container.Instance.Resolve<IGoodsService>().Get(int.Parse(Request["GoodsId"]));
                newp.InnQuantity = int.Parse(Request["Quantity"]);
                purchaseOrder.PurchaseOrderDetails.Add(newp);
                //进货数量和预计金额计算(保存时计算)
                purchaseOrder.Quantity = 0;
                purchaseOrder.Money1 = 0;
                

                Container.Instance.Resolve<IPurchaseOrderService>().Update(purchaseOrder);
                return View(purchaseOrder);
            }
            //删除
            if (pd == 2)
            {
                string str = Request["Ids"];
                //根据ID查询信息
                PurchaseOrderDetails xxx = Container.Instance.Resolve<IPurchaseOrderDetailsService>().Get(int.Parse(Request["xxx"]));
                Container.Instance.Resolve<IPurchaseOrderDetailsService>().Delete(xxx);

                return View(Container.Instance.Resolve<IPurchaseOrderService>().Get(model.ID));
            }
            //保存时如果无货物则删除订单
            if (pd == 3)
            {
                if (model.ID != 0)
                {
                    purchaseOrder = Container.Instance.Resolve<IPurchaseOrderService>().Get(model.ID);
                    if (purchaseOrder.PurchaseOrderDetails.Count() == 0)//判断有无货物（无则删除有则进行种数和金额计算）
                    {
                        Container.Instance.Resolve<IPurchaseOrderService>().Delete(model.ID);
                    }
                    else
                    {
                        foreach (var item in purchaseOrder.PurchaseOrderDetails)
                        {
                            purchaseOrder.Quantity += 1;
                            purchaseOrder.Money1 += (item.GoodsId.InnPrice * item.InnQuantity);
                            purchaseOrder.Money2 = purchaseOrder.Money1;
                        }
                        purchaseOrder.Remarks = "无";
                        Container.Instance.Resolve<IPurchaseOrderService>().Update(purchaseOrder);//跟新数据
                    }
                }
                return RedirectToAction("Index");
            }
            return View();

        }

        //详情删除
        public ActionResult Deletes( int id)
        {
            //根据ID查询信息
            PurchaseOrderDetails model = Container.Instance.Resolve<IPurchaseOrderDetailsService>().Get(id);
            Container.Instance.Resolve<IPurchaseOrderDetailsService>().Delete(model);

            ViewBag.ID = model.PurchaseOrder.ID;

            //跳转到Create页面并返回当前进货单
            return View("Create",Container.Instance.Resolve<IPurchaseOrderService>().Get(model.PurchaseOrder.ID));
            //return vieRedirectToAction
        }
        #endregion


        #region 删除
        public ActionResult Delete(int id)
        {
            //根据ID查询信息
            PurchaseOrder model = Container.Instance.Resolve<IPurchaseOrderService>().Get(id);
            Container.Instance.Resolve<IPurchaseOrderService>().Delete(model);

            //跳转到Index页面
            return RedirectToAction("Index");
        }
        #endregion


        #region 订单申请（显示待分配订单）
        public ActionResult Application()
        {

            IList<PurchaseOrder> list = Container.Instance.Resolve<IPurchaseOrderService>().GetAll().Where(m => m.Status=="待分配").ToList();
            return View(list);
        }
        #endregion


        #region 订单申请ing
        public ActionResult Applicationing(int id)
        {
            //根据ID查询信息
            PurchaseOrder model = Container.Instance.Resolve<IPurchaseOrderService>().Get(id);
            model.UserIds = AppHelper.LoginedUser;//修改进货员
            model.Status = "进货中";//修改状态
            Container.Instance.Resolve<IPurchaseOrderService>().Update(model);

            return RedirectToAction("Application");
        }
        #endregion


        #region 登录用户拥有订单
        public ActionResult Own(string modules)
        {
            //准备查询条件
            List<ICriterion> newPurchaseOrder = new List<ICriterion>();
            newPurchaseOrder.Add(Expression.Eq("UserIds.ID", AppHelper.LoginedUser.ID));
            //判断是否为空
            if (!string.IsNullOrEmpty(modules))
            {
                if (modules != "1")
                { newPurchaseOrder.Add(Expression.Eq("Status", modules)); }
            }

            IList<PurchaseOrder> list = Container.Instance.Resolve<IPurchaseOrderService>().GetAll(newPurchaseOrder);
            return View(list);
        }
        #endregion


        #region 订单完成
        public ActionResult Done(int id)
        {
            //根据ID查询信息
            PurchaseOrder model = Container.Instance.Resolve<IPurchaseOrderService>().Get(id);
            model.Status = "结束";//修改状态

            Goods goods = new Goods();
            //库存更新
            foreach (var item in model.PurchaseOrderDetails)
            {
                goods = Container.Instance.Resolve<IGoodsService>().Get(item.GoodsId.ID);
                goods.Stock+=item.InnQuantity;
                Container.Instance.Resolve<IGoodsService>().Update(goods);
            }

            //创建账单
            Bill bill = new Bill();

            //获取最后一条账单记录
            Bill lastBill = Container.Instance.Resolve<IBillService>().GetAll().Last();
            //赋值
            bill.Money = model.Money2;
            bill.Remarks = "进货";
            bill.Balance = lastBill.Balance-model.Money2;//进货后余额
            bill.UserId = model.UserIds;
            bill.Time = DateTime.Now;
            //创建新账单
            Container.Instance.Resolve<IBillService>().Create(bill);

            Container.Instance.Resolve<IPurchaseOrderService>().Update(model);

            return RedirectToAction("Own");
        }
        #endregion


        #region 详情
        public ActionResult Xq(int id)
        {

            return View(Container.Instance.Resolve<IPurchaseOrderService>().Get(id).PurchaseOrderDetails);
        }
        #endregion
    }
}