using ESCS.Core;
using ESCS.Domain;
using ESCS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESCS.Web.Controllers
{
    public class ProfitingController : BaseController
    {
        public void Allprice()
        {
            IList<SalesOrder> lists = Container.Instance.Resolve<ISalesOrderService>().GetAll().Where(o=>o.Status=="完成").ToList().Where(o=>o.Remarks=="购买").ToList();
            if (lists .Count()!= 0)
            {
                ViewBag.first = lists.FirstOrDefault().Time;//开始时间
                ViewBag.last = lists.LastOrDefault().Time;//结束时间
                double b = 0, c = 0, d = 0, e = 0;
                foreach (var item in lists)
                {
                    b += item.Money;//总营业额
                    c += item.Profits;//总利润
                    d = item.Profits / item.Money;//利率
                    e += 1;//单数
                }
                ViewBag.B = b;
                ViewBag.C = c;
                ViewBag.D = d;
                ViewBag.E = e;
            }
            
        }
        [HttpGet]
        public ActionResult Index()
        {
            Allprice();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string hitime = "", string lotime = "")
        {
            Allprice();//所有盈利
            hitime = hitime.Replace("T", " ").Replace("-", "/");
            lotime = lotime.Replace("T", " ").Replace("-", "/");
            //指定时间间隔盈利
            if (Container.Instance.Resolve<ISalesOrderService>().Time(hitime) && Container.Instance.Resolve<ISalesOrderService>().Time(lotime))//判断时间格式
            {
                ////查询指定索引页的的商品信息
                IList<SalesOrder> list = Container.Instance.Resolve<ISalesOrderService>().BetweenTime(hitime, lotime);
                //传递数据
                double a = 0, f = 0, g = 0, h = 0;
                foreach (var item in list)
                {
                    a += item.Money;//总营业额
                    f += item.Profits;//总利润
                    g = item.Profits / item.Money;//利率
                    h += 1;//单数
                }
                ViewBag.A = a;
                ViewBag.F = f;
                ViewBag.G = g;
                ViewBag.H = h;

                //返回时间
                ViewBag.Hvalue = hitime;
                ViewBag.Lvalue = lotime;

                return View();
            }
            return View();
        }
	}
}