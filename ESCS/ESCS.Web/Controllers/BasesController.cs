using ESCS.Web.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESCS.Web.Controllers
{
    public class BasesController : Controller
    {
        //重写OnActionExecuting方法，这个方法在Action执行时执行
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Customers();
            //base.OnActionExecuting(filterContext);

            string controllerName = filterContext.Controller.ToString();//获取控制器名称
            string actionName = filterContext.ActionDescriptor.ActionName;//获取Action的名称
            //如果是用户请求登录页面则不验证权限
            if ((controllerName.Equals("ESCS.Web.Controllers.EasyShoppingCentreController") && (actionName == "YiGouShangCheng" || actionName == "Create" || actionName == "Goods")) || (controllerName.Equals("ESCS.Web.Controllers.CustomerController") && (actionName == "CustomerLogin")))
            {
                if (ESCS.Web.Apps.AppHelper.LoginedCustomer != null)
                {Customers(); }
                
                base.OnActionExecuting(filterContext);//执行mvc默认的行为
            }
            else
            {
                //if (controllerName.Equals("ESCS.Web.Controllers.EasyShoppingCentreController") && (actionName == "Goods"))//如果打开商品界面专挑到商城页面
                //{
                //    filterContext.HttpContext.Response.Redirect("~/EasyShoppingCentre/YiGouShangCheng?ReturnUrl=" + Server.UrlEncode(Request.Url.OriginalString), true);
                //}
                if (ESCS.Web.Apps.AppHelper.LoginedCustomer == null)//如果登录信息丢失
                {
                    Redirect(filterContext);//跳转到登录页面
                    Response.End();//结束响应
                }
                else
                {
                    Customers();//设置动态属性，用于在母版页上显示菜单
                    base.OnActionExecuting(filterContext);//执行mvc默认的行为
                }
            }
        }

        /// <summary>
        /// 获取当前用户功能模块
        /// </summary>
        public void Customers()
        {

                ViewBag.pd = 1;
                ViewBag.name = AppHelper.LoginedCustomer.CustomerName;
                ViewBag.Picture = AppHelper.LoginedCustomer.Picture;
        }


        //页面跳转
        private void Redirect(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect("~/Customer/CustomerLogin?ReturnUrl=" + Server.UrlEncode(Request.Url.OriginalString), true);
        }
	}
}