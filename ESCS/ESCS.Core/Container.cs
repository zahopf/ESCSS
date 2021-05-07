using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor;
using Castle.MicroKernel;
using Castle.Core.Resource;
using Castle.DynamicProxy;
using System.Reflection;
using System.Xml.Linq;

namespace ESCS.Core
{
    public class Container
    {
        private static Container instance;
        private static XDocument xml;//UI层的服务实现配置文件实例

        public static Container Instance//全局IOC窗口实例，单例模式
        {
            get
            {
                if (instance == null)
                {
                    instance = new Container();
                }
                if (xml == null)
                {
                    //读取UI层的服务实现配置文件
                    xml = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/Configuration/LMSConfiguration.xml");
                }
                return instance;
            }
        }

        public T Resolve<T>()
            where T : class
        {
            return GetComponentInstance<T>(typeof(T).Name);
        }

        public T GetComponentInstance<T>(string service)
            where T:class
        {
            if (xml == null)
            {

            }
            //在UI层的服务实现配置文件根据接口文件名读取实现接口文件的业务逻辑实体
            var element = (from p in xml.Root.Elements("element")
                           where p.Attribute("Id").Value.Equals(service)
                           select new
                           {
                               serviceNamespace = p.Attribute("Service").Value,
                               classNamespace = p.Attribute("Class").Value
                           }).FirstOrDefault();
            if (string.IsNullOrEmpty(element.classNamespace))
            {
                throw new Exception("配置文件结点" + service + "出错！");
            }
            string[] configs = element.classNamespace.Split(',');
            if (configs.Length != 2)
            {
                throw new Exception("配置文件结点" + service + "出错！");
            }

            ProxyGenerator generator = new ProxyGenerator();//代码生成器
            T t = (T)Assembly.Load(configs[1]).CreateInstance(configs[0]);//根据配置文件实例化一个对象
            return t;
        }

        public void Dispose()
        {
            instance = null;
            xml = null;
        }
    }
}
