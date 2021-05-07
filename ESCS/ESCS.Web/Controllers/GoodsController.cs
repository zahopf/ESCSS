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
    public class GoodsController : BaseController
    {
        #region 商品显示
        public ActionResult Index(string goodsName)
        {
            //准备查询条件
            List<ICriterion> newGoods = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(goodsName))
            {
                newGoods.Add(Expression.Eq("GoodsName", goodsName));
            }

            IList<Goods> list = Container.Instance.Resolve<IGoodsService>().GetAll(newGoods);
            foreach (var item in list)
            {
                if (item.Stock <= 100)
                {
                    item.Status = "缺货";
                    Container.Instance.Resolve<IGoodsService>().Update(item);
                }
                else
                {
                    item.Status = "货源充足";
                    Container.Instance.Resolve<IGoodsService>().Update(item);
                }
            }
            return View(list);
        }
        #endregion

        #region 添加商品
        [HttpGet]
        public ActionResult Create()//这个方法用于mvc展示页面
        {
            Goods goods = new Goods();//实体化一个空的实体

            ViewBag.kindList = Container.Instance.Resolve<IGoodsKindService>().GetAll().Where(it => it.ParentID != null).ToList();

            return View(goods);//用一个视图展示goods
        }

        [HttpPost]
        public ActionResult Create(Goods model, int KindName)
        {
            //重新传入商品种类
            ViewBag.KindList = Container.Instance.Resolve<IGoodsKindService>().GetAll().Where(it => it.ParentID != null).ToList();

            //移除不验证项
            ModelState.Remove("IsActive");
            ModelState.Remove("GoodsKindId");
            ModelState.Remove("Picture");

            //模型验证
            if (ModelState.IsValid)
            {
                model.Picture = AppHelper.Picture;
                model.Discount = 1;//初始化折扣
                model.IsActive = true;
                model.Status = ".";
                model.GoodsKindId = Container.Instance.Resolve<IGoodsKindService>().Get(KindName);//种类

                List<ICriterion> goodsName = new List<ICriterion>();
                goodsName.Add(Expression.Eq("GoodsName", model.GoodsName));

                if (Container.Instance.Resolve<IGoodsService>().Exists(goodsName))
                {
                    ModelState.AddModelError("GoodsName", "此名字已存在");
                    return View(model);
                }
                else
                {
                    Container.Instance.Resolve<IGoodsService>().Create(model);
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        #endregion


        #region 注销激活
        public ActionResult SwitchStatus(int id)
        {
            Container.Instance.Resolve<IGoodsService>().SwitchStatus(id);//调用业务逻辑层方法，切换用户状态
            return RedirectToAction("Index");//跳转到列表页面
        }
        #endregion


        #region 删除商品
        public ActionResult Delete(int id)
        {
            //准备查询条件
            List<ICriterion> newGoods = new List<ICriterion>();
            newGoods.Add(Expression.Eq("GoodsId.ID", id));

            //先删除购物车内商品
            IList<ShoppingCart> ss= Container.Instance.Resolve<IShoppingCartService>().GetAll(newGoods);
            IList<SalesOrderDetails> sss = Container.Instance.Resolve<ISalesOrderDetailsService>().GetAll(newGoods);
            IList<PurchaseOrderDetails> pp = Container.Instance.Resolve<IPurchaseOrderDetailsService>().GetAll(newGoods);
            
            if (ss.Count() > 0)
            {
                foreach (var item in ss)
                {
                    Container.Instance.Resolve<IShoppingCartService>().Delete(item.ID);
                }
            }
            if (sss.Count() > 0)
            {
                foreach (var item in sss)
                {
                    Container.Instance.Resolve<ISalesOrderService>().Delete(item.ID);
                }
            }
            if (pp.Count() > 0)
            {
                foreach (var item in pp)
                {
                    Container.Instance.Resolve<IPurchaseOrderDetailsService>().Delete(item.ID);
                }
            }

            Container.Instance.Resolve<IGoodsService>().Delete(id);
            return RedirectToAction("Index");//跳转到列表视图
        }
        #endregion

        #region 修改基本信息
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //根据商品id查询商品
            Goods model = Container.Instance.Resolve<IGoodsService>().Get(id);
            //传入商品种类
            ViewBag.KindList = Container.Instance.Resolve<IGoodsKindService>().GetAll().Where(it => it.ParentID != null).ToList();
            //调用创建视图
            return View("Create", model);
        }
        [HttpPost]
        public ActionResult Edit(Goods model, int KindName)
        {
            ViewBag.KindList = Container.Instance.Resolve<IGoodsKindService>().GetAll().Where(it => it.ParentID != null).ToList();

            Goods goods = Container.Instance.Resolve<IGoodsService>().Get(model.ID);

            //移除不验证项
            ModelState.Remove("IsActive");
            ModelState.Remove("Discount");
            ModelState.Remove("GoodsKindId");

            if (ModelState.IsValid)
            {
                goods.Discount = model.Discount;
                goods.GoodsKindId = Container.Instance.Resolve<IGoodsKindService>().Get(KindName);//种类
                goods.GoodsName = model.GoodsName;
                goods.InnPrice = model.InnPrice;
                goods.OutPrice = model.OutPrice;
                goods.Stock = model.Stock;

                List<ICriterion> goodsName = new List<ICriterion>();
                goodsName.Add(Expression.Eq("GoodsName", model.GoodsName));
                //名字相同通过
                if (Container.Instance.Resolve<IGoodsService>().Get(model.ID).GoodsName == model.GoodsName)
                {
                    Container.Instance.Resolve<IGoodsService>().Update(goods);
                    return RedirectToAction("Index");
                }
                //不同验则证纯在
                if (Container.Instance.Resolve<IGoodsService>().Exists(goodsName))
                {
                    ModelState.AddModelError("GoodsName", "此名字已存在");
                    return View("Create", model);
                }
                else
                {
                    Container.Instance.Resolve<IGoodsService>().Update(goods);
                    return RedirectToAction("Index");
                }
            }
            return View("Create", model);

        }
        #endregion


        //#region 销量统计
        //public ActionResult SalesStatistics()
        //{
        //    IList<SalesOrderDetails> lists = Container.Instance.Resolve<ISalesOrderDetailsService>().GetAll();
        //    IList<Goods> list = Container.Instance.Resolve<IGoodsService>().GetAll();
        //    foreach (var item in list)
        //    {
 
        //    }

        //    return View();
        //}
        //#endregion


	}
}