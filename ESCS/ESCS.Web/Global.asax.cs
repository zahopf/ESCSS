using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ESCS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            IConfigurationSource source = System.Configuration.ConfigurationManager.GetSection("activerecord") as IConfigurationSource;


            if (!ActiveRecordStarter.IsInitialized)//如果ActiveRecordStarter框架没有初始化
            {
                ActiveRecordStarter.Initialize(typeof(ESCS.Domain.User).Assembly, source);//初始化程序信
            }
        }
    }
}
