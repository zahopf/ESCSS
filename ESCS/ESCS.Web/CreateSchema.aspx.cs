using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord;
using ESCS.Core;
using ESCS.Service;
using ESCS.Domain;
using ESCS.Manager;
using ESCS.Component;

namespace ZDSoft.LMS.Web
{
    public partial class CreateSchema : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                //删除数据库
                ActiveRecordStarter.DropSchema();
                //创建数据库存
                
                              

                if (!ActiveRecordStarter.IsInitialized)//如果ActiveRecordStarter框架没有初始化
                {
                    IConfigurationSource source2 = System.Configuration.ConfigurationManager.GetSection("activerecord") as IConfigurationSource;//初始化配置文件                    
                    ActiveRecordStarter.Initialize(typeof(ESCS.Domain.User).Assembly, source2);//初始化程序信
                }
                ActiveRecordStarter.CreateSchema();//通过程序集创建数据库
                //ActiveRecordStarter.CreateSchemaFromFile(Server.MapPath("~/Content/sql/lms.sql"));//通过脚本文件创建 
                Response.Write("1、生成数据库成功！（1/2)...<br />");//提示用户，生成数据库成功
                //Container.Instance.Resolve<ISystemSettingService>().InitDataBase();
                //Response.Write("2、创建基础数据成功（2/2）...<br />");//提示用户，创建基础数据成功
            }
            catch (Exception ex)
            {
                throw ex;
                //Response.Write("************"+ex.Message);
            }
        }

    }
}