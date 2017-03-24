using BookShop.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShop.Models
{
    //本过滤器是用来验证后台管理员登录
    public class MyValidateForCheckAdmin:FilterAttribute,IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //取出保持在session里的变量，名字叫adminname的值

            if (HttpContext.Current.Session["adminname"] == null)
            {
                var result = new ContentResult();
                result.Content = "<script>alert('必须先登录才能访问');location.href='/user/login';</script>";
                filterContext.Result =  result;
              //  HttpContext.Current.Response.Write("<script>alert('sdf');</script>");//写入客户端
             //   filterContext.HttpContext
               // HttpContext.Current.Response.End();//结束请求，发送响应流到客户端
            }

        }
    }
}