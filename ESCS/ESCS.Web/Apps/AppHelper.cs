using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESCS.Domain;

using ESCS.Service;
using ESCS.Core;

namespace ESCS.Web.Apps
{
    public class AppHelper
    {
        //使用MD5加密传入的字符
        public static string EncodeMd5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
        }

        //保存图片地址
        public static string Picture
        {
            get
            {
                //如果Session信息不为空
                if (HttpContext.Current.Session["Picture"] != null)
                {
                    //将Session登录信息转换为User并返回
                    return HttpContext.Current.Session["Picture"] as string;
                }
                return "";
            }
            set
            {
                //将传入的信息保存到Session中
                HttpContext.Current.Session["Picture"] = value;
            }
        }

        //保存登录用户的信息
        public static User LoginedUser
        {
            get
            {
                //如果Session信息不为空
                if (HttpContext.Current.Session["LoginedUser"] != null)
                {
                    //将Session登录信息转换为User并返回
                    return HttpContext.Current.Session["LoginedUser"] as User;
                }
                return null;
            }
            set
            {
                //将传入的信息保存到Session中
                HttpContext.Current.Session["LoginedUser"] = value;
            }
        }


        //保存登录客户的信息
        public static Customer LoginedCustomer
        {
            get
            {
                //如果Session信息不为空
                if (HttpContext.Current.Session["LoginedCustomer"] != null)
                {
                    //将Session登录信息转换为Customer并返回
                    return HttpContext.Current.Session["LoginedCustomer"] as Customer;
                }
                return null;
            }
            set
            {
                //将传入的信息保存到Session中
                HttpContext.Current.Session["LoginedCustomer"] = value;
            }
        }

        //获取主机的地址
        public static string Host
        {
            get
            {
                return HttpContext.Current.Request.Url.AbsoluteUri;
            }
        }


        //<summary>
        //当前登录用户的功能模块（菜单） 
        //</summary>       
        public static IList<SystemFunction> Privileges
        {
            get
            {
                //获取系统中所有的功能模块
                IList<SystemFunction> privileges = Container.
                    Instance.
                    Resolve<ISystemFunctionService>().GetAll();
                if (privileges == null)
                {
                    return privileges;
                }
                IList<SystemFunction> privilegeList = null;//声明一个集合变量
                if (LoginedUser != null && LoginedUser.RoleList != null)//如果有用户登录并且有对应的角色
                {
                    foreach (Role role in LoginedUser.RoleList)//遍历当前用户的角色
                    {
                        if (role != null)
                        {
                            foreach (SystemFunction function in role.SysFunctionList)//获取当前用户可以操作的功能
                            {
                                if (privilegeList == null)
                                {
                                    privilegeList = new List<SystemFunction>();//实例化权限集合
                                }
                                if (privilegeList.Where(o => o.ID == function.ID).Count() < 1)//如果当前功能在权限集合中不存在
                                {
                                    privilegeList.Add(function);//添加到集合中
                                }
                            }
                            return privilegeList;
                        }
                    }
                }
                return privilegeList;
            }

        }
    }
}