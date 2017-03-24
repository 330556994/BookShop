using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
namespace BookShop.Models
{
    /// <summary>
    /// 自定义过滤器，作用在action上
    /// 其实过滤器就是一个特性类，所以要继承 FilterAttribute 
    /// IActionFilter 接口，过滤器接口
    /// </summary>
    public class MyFielterAction:FilterAttribute,IActionFilter
    {
        /// <summary>
        ///  调用动作之后执行
        /// </summary>
        /// <param name="filterContext">请求上下文</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var context = filterContext.HttpContext;
            context.Response.Write("<p>OnActionExecuted方法执行了</p>");
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 调用动作之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            context.Response.Write("<p>OnActionExecuting方法执行了</p>");
            //throw new NotImplementedException();
        }
    }
}