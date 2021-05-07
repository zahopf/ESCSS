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
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            IList<User> lists = Container.Instance.Resolve<IUserService>().GetAll();
            ViewBag.count = lists.Count();

            return View(lists);
        }

        public ActionResult Indexs()
        {
            return View();
        }
	}
}