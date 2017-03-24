using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BookShop.Models
{
    /// <summary>
    /// 演示过滤器，作用在视图上
    /// </summary>
    public class MyFilterView:FilterAttribute,IResultFilter
    {
        /// <summary>
        /// 调用视图结束后执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var context = filterContext.HttpContext;
            context.Response.Write("<p>OnResultExecuted方法执行了</p>");
    
        }
        /// <summary>
        /// 调用视图之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            context.Response.Write("<p>OnResultExecuting方法执行了</p>");
  
        }
    }
}