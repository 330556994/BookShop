using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BookShop.Models
{
    /// <summary>
    /// 防sql注入过滤器
    /// </summary>
    public class MySqlValidate:FilterAttribute,IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //读取web.config中配置的防SQL注入字符串
            //  ',-
            string pattern = System.Configuration.ConfigurationManager
                .AppSettings["sqlCode"];
            string[] parts = pattern.Split(',');//按照逗号分隔
            //数据提交的途径有哪些？
            // post,get， 路由表
            //先处理get请求
            var queryString = filterContext.HttpContext.Request.QueryString;
            foreach (string key in queryString.AllKeys) {
                foreach (string code in parts) {
                    if (queryString[key].Contains(code) == true) { 
                        //这个键值中，包含敏感字符的话
                        filterContext.Result = new RedirectResult("~/sqlerr.html");
                        //filterContext.HttpContext.Response.Redirect("~/sqlerr.html");
                        //filterContext.HttpContext.Response.End();//结束请求
                    }
                }
            }
            //下面验证post请求的数据
            var postString = filterContext.HttpContext.Request.Form;
            foreach (string key in postString.AllKeys)
            {
                foreach (string code in parts)
                {
                    if (postString[key].Contains(code) == true)
                    {
                        //这个键值中，包含敏感字符的话
                        filterContext.Result = new RedirectResult("~/sqlerr.html");
                        //filterContext.HttpContext.Response.Redirect("~/sqlerr.html");
                        //filterContext.HttpContext.Response.End();//结束请求
                    }
                }
            }
            //下面验证路由数据
            var routeString = filterContext.RouteData;
            foreach (string value in routeString.Values.Values)
            {
                foreach (string code in parts)
                {
                    if (value.Contains(code) == true)
                    {
                        //设置了result之后，不会进入action
                        filterContext.Result = new RedirectResult("~/sqlerr.html");
                        //这个键值中，包含敏感字符的话
                       // filterContext.HttpContext.Response.Redirect("~/sqlerr.html");
                        //filterContext.HttpContext.Response.End();//结束请求
                    }
                }
            }


        }
    }
}