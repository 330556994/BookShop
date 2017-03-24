using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BookShop.Models.Entities;
using BookShop.Models.Services;

namespace BookShop
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    /// <summary>
    ///全局应用程序类 
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 应用程序启动时调用,第一个访客访问时运行
        /// </summary>
        protected void Application_Start()
        {


            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["online"] = 0;//创建一个application级别变量,表示总在线人数
            int total=123;//这里应该是从数据库或文件中读取总访问量
            Application["total"] = total;//创建全局变量，表示总访问量
            //创建users全局变量，保存的是集合对象，表示在线会员列表
            Application["users"] = new List<string>();
          



        }
        protected void Application_End() { 
        }
        
        /// <summary>
        /// session启动时调用
        /// </summary>
        protected void Session_Start() {
            Session.Timeout = 30;//设置超时时间为60分钟,默认30分钟
            //读取客户端的名字为accpname的cookie
            HttpCookie cookie = Request.Cookies["_accp_name"];
            List<string> users = null; //定义表示在线帐号的集合变量
            if (cookie != null) { 
            //cookie取到了
                string name = cookie.Value;//获得保存的用户名
                //需要再次验证该用户的合法性
                Session["name"] = name;//重建session变量,会员登录
                /////////////////////////////////////////////////////////////////////
                //这里的功能是，登录成功后，帮当前帐号添加到全局变量里
                Application.Lock();
                users = Application["users"]
                    as List<string>;
                users.Add(name);
                Application.UnLock();
                ////////////////////////////////////////////////////////////////////

            }

            ////////////////////////////////////////
            // 会话启动，总在线和总访问量自增
            //得到总在线
            //有个问题，当同时访问时，会产生并发，产生数量不准确
            Application.Lock();//先锁定
            int online = Convert.ToInt32(Application["online"]);
            //得到总访问量
            int total = Convert.ToInt32(Application["total"]);
            online++;
            total++;
            //再保存
            Application["online"] = online;
            Application["total"] = total;
            Application.UnLock();//解锁
            ////////////////////////////////////////////////////////////////
            // 开始初始化购物车
          //  Session["car"] = new List<Item>();
            ItemService itemservice = new ItemService();
            itemservice.LoadCarFromCookie();//从cookie里恢复购物车，如果不存在，则会创建空的集合


        }
        /// <summary>
        /// 会话销毁后运行
        /// </summary>
        protected void Session_End()
        {
            //会话销毁时，总在线减一
            Application.Lock();//先锁定
            int online = Convert.ToInt32(Application["online"]);
          
            online--;
      
            //再保存
            Application["online"] = online;
       
            Application.UnLock();//解锁

        }
    }
}