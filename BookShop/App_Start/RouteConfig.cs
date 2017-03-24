using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //演示 路由1
            routes.MapRoute(
                name: "route1",
                url: "{year}-{month}-{day}",//匹配URl
                defaults: new
                {
                    controller = "demo",
                    action = "demo6",
                    day = UrlParameter.Optional //可选的
                }
                , namespaces: new string[] { "BookShop.Controllers" }
                //路由参数约束，正则表达式
                , constraints: new { day=@"\d{2}",year="\\d{4}",month="\\d{2}" }
            );
            //路由演示2
            routes.MapRoute(
               name: "route2",
               url: "mybooks/{id}.html",//匹配URl
               defaults: new
               {
                   controller = "books",
                   action = "detail"
               //    id = UrlParameter.Optional //可选的
               }
               , namespaces: new string[] { "BookShop.Controllers" }
           );



            //routes 路由集合
            //注册一个路由，这是系统默认配置的
            routes.MapRoute(
                name: "Default", //路由名称
                //{controller}/{action}/{id} 
                //{占位符} / 字面量
                // 固定占位两个{controller}{action},占位符之间必须要有字面量

                url: "{controller}/{action}/{id}",//匹配URl
                defaults: new { controller = "Home", action = "Index",
                   id = UrlParameter.Optional //可选的
                }
                , namespaces: new string[] { "BookShop.Controllers" }
            );
        }
    }
}