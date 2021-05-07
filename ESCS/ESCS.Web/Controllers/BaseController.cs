using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESCS.Web.Apps;

namespace ESCS.Web.Controllers
{
    public class BaseController : Controller
    {

        //重写OnActionExecuting方法，这个方法在Action执行时执行
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //GetCurrentUserPrivilege();
            //base.OnActionExecuting(filterContext);

            string controllerName = filterContext.Controller.ToString();//获取控制器名称
            string actionName = filterContext.ActionDescriptor.ActionName;//获取Action的名称
            //如果是用户请求登录页面则不验证权限
            if ((controllerName.Equals("ESCS.Web.Controllers.UserController") && (actionName == "Login" || actionName == "BackPassword")) || (controllerName.Equals("ESCS.Web.Controllers.CustomerController") && (actionName == "CustomerLogin" || actionName == "Create")))
            {
                base.OnActionExecuting(filterContext);//执行mvc默认的行为
            }
            else
            {
                if (ESCS.Web.Apps.AppHelper.LoginedUser == null)//如果登录信息丢失
                {
                    Redirect(filterContext);//跳转到登录页面
                    Response.End();//结束响应
                }
                else
                {
                    GetCurrentUserPrivilege();//设置动态属性，用于在母版页上显示菜单
                    base.OnActionExecuting(filterContext);
                }
            }
        }

        /// <summary>
        /// 获取当前用户功能模块
        /// </summary>
        private void GetCurrentUserPrivilege()
        {
            ViewBag.PrivilegeList = ESCS.Web.Apps.AppHelper.Privileges;
        }


        //页面跳转
        private void Redirect(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect("~/User/Login?ReturnUrl=" + Server.UrlEncode(Request.Url.OriginalString), true);
        }
    }
}