using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Services;

namespace BookShop.Models
{
    /// <summary>
    /// 本过滤器是用来完成前台身份验证
    /// </summary>
    public class MyValidate:FilterAttribute,IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["name"] == null)
            {
                string html = string.Format("<script>alert('必须先登录才能访问');"
                + "location.href='/user/login?returnurl={0}';</script>"
                , HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"]);
                var result = new ContentResult();//创建一个内容输出结果
                result.Content = html;
                filterContext.Result = result; //设置当前请求上下文的输出返回,这样action就不会执行了

              //  HttpContext.Current.Response.Write(html);//写入客户端
                //Response.End();//结束请求 ,还是会执行那个action
                //HttpContext.Current.Response.End();//结束请求，发送响应流到客户端
            }
        }
    }
}