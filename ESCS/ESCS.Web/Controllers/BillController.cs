using System;
using ESCS.Core;
using ESCS.Domain;
using ESCS.Service;
using ESCS.Web.Apps;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESCS.Web.Controllers
{
    public class BillController : BaseController
    {
        public ActionResult Index(string Remarks)
        {
            //组织查询条件
            IList<ICriterion> queryConditions = new List<ICriterion>();
            if (!string.IsNullOrEmpty(Remarks))
            {
                //根据备注
                queryConditions.Add(Expression.Eq("Remarks", Remarks));
            }

            IList<Bill> lists = Container.Instance.Resolve<IBillService>().GetAll(queryConditions);
            return View(lists);
        }
    }
}