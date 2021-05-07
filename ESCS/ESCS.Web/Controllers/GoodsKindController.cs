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
    public class GoodsKindController : BaseController
    {
        #region 商品种类显示
        public ActionResult Index(string kindName)
        {
            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            //判断是否为空
            if (!string.IsNullOrEmpty(kindName))
            {
                newKind.Add(Expression.Eq("KindName", kindName));
            }

            IList<GoodsKind> lists = Container.Instance.Resolve<IGoodsKindService>().GetAll(newKind).Where(m => m.ParentID == null).ToList(); 
            return View(lists);
            
        }
        #endregion


        #region 创建种类
        [HttpGet]
        public ActionResult Create()
        {
            //定义一个新对象传入视图
            GoodsKind role = new GoodsKind();
            return View(role);
        }

        [HttpPost]
        public ActionResult Create(GoodsKind kind)
        {
            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            newKind.Add(Expression.Eq("KindName", kind.KindName));
            //判断是否未输入
            if (kind.KindName == null)
            {
                ModelState.AddModelError("KindName", "不能为空");
                return View(kind);
            }
            //重复检测
            if (Container.Instance.Resolve<IGoodsKindService>().Exists(newKind))
            {
                ModelState.AddModelError("KindName", "此名字已存在");
                return View(kind);
            }
            else
            {
                //创建新种类
                Container.Instance.Resolve<IGoodsKindService>().Create(kind);
                Response.Write("<script>opener.location.href=opener.location.href;window.opener=null;window.close();</script>");//刷新当前页并关闭弹出页
                return View();
            }
        }
        #endregion


        #region 删除
        public ActionResult Delete(int id)
        {
            //根据ID查询种类信息
            GoodsKind model = Container.Instance.Resolve<IGoodsKindService>().Get(id);

            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            newKind.Add(Expression.Eq("ParentID.ID", id));
            IList<GoodsKind> lists = Container.Instance.Resolve<IGoodsKindService>().GetAll(newKind);

            //判断一级种类是否已分配了二级种类和商品，如果有，则不能删除
            if (lists.Count == 0)
                Container.Instance.Resolve<IGoodsKindService>().Delete(model);

            //跳转到Index页面
            return RedirectToAction("Index");
        }
        #endregion


        #region 修改
        [HttpGet]
        public string Edit(int id)
        {
            //根据id获取实体
            GoodsKind model = Container.Instance.Resolve<IGoodsKindService>().Get(id);
            return model.KindName;
        }
        [HttpPost]
        public string Edit(string name,int id )
        {
            GoodsKind newKind = Container.Instance.Resolve<IGoodsKindService>().Get(id);

            List<ICriterion> kindName = new List<ICriterion>();
            kindName.Add(Expression.Eq("KindName", name));

            //如修改判断是否存在再修改，若果未修改则保存原来信息
            if (Container.Instance.Resolve<IGoodsKindService>().Get(id).KindName == name || !Container.Instance.Resolve<IGoodsKindService>().Exists(kindName))
            {
                newKind.KindName = name;
                Container.Instance.Resolve<IGoodsKindService>().Update(newKind);
                return "修改成功！！！";
               
            }
            else
            {
                return "名字已存在！！！";
            }
        }
        #endregion


        #region 二级分类
        public ActionResult Kinds(int id, string kindName="")
        {
            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            if (!string.IsNullOrEmpty(kindName))
            {
                newKind.Add(Expression.Eq("KindName", kindName));
            }

            //根据父亲id查询
            newKind.Add(Expression.Eq("ParentID.ID", id));

            IList<GoodsKind> lists = Container.Instance.Resolve<IGoodsKindService>().GetAll(newKind);

            ViewBag.father = Container.Instance.Resolve<IGoodsKindService>().Get(id).KindName;
            ViewBag.OneKind = id;

            return View(lists);
        }
        #endregion


        #region 二级分类创建
        [HttpPost]
        public string CreateKinds(int oneKind,string name)
        {
            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            newKind.Add(Expression.Eq("KindName", name));
            //判断是否未输入
            if (name == null)
            {
                return "不能为空！！！";
            }
            //重复检测
            if (Container.Instance.Resolve<IGoodsKindService>().Exists(newKind))
            {
                return "该种类已存在，创建失败！！！";
            }
            else
            {
                //创建新种类
                GoodsKind model = new GoodsKind();
                model.ParentID = Container.Instance.Resolve<IGoodsKindService>().Get(oneKind);
                model.KindName = name;
                Container.Instance.Resolve<IGoodsKindService>().Create(model);
                return "创建成功！！！";
            }

            
        }
        #endregion


        #region 删除二级分类
        public ActionResult Deletes(int id, int oneKind)
        {
            //根据ID查询种类信息
            GoodsKind model = Container.Instance.Resolve<IGoodsKindService>().Get(id);

            //准备查询条件
            List<ICriterion> newKind = new List<ICriterion>();
            newKind.Add(Expression.Eq("GoodsKindId.ID", model.ID));
            

            //判断种类是否已分配了商品，如果有，则不能删除
            if (Container.Instance.Resolve<IGoodsService>().GetAll(newKind).Count == 0)
                Container.Instance.Resolve<IGoodsKindService>().Delete(model);

            return RedirectToAction("Kinds", new { id = oneKind });
        }
        #endregion
	}
}