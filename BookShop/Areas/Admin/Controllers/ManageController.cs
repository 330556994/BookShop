using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;
namespace BookShop.Areas.Admin.Controllers
{
    //在控制器上添加这个过滤器，表示当有异常发生，去调用某个视图
    [HandleError(ExceptionType=typeof(Exception),View="error")]
    public class ManageController : Controller
    {
        //
        // GET: /Admin/Manage/
        [Authorize] //加这个特性，就是启用验证
        public ActionResult main()
        {
            return View();
        }
        public ActionResult Login() {
         
            return View();
        }
        [HttpPost]
        public ActionResult  Login(sysman man)
        {
            if (this.ModelState.IsValid == true)
            {
                using (BookShopPlusEntities db = new BookShopPlusEntities())
                {
                    var sysman = db.sysman.SingleOrDefault(p => p.name == man.name
                                && p.pwd == man.pwd);
                    if (sysman != null)
                    {
                        //用户名和 密码正确
                        //本方法的作用是，创建一个cookie的对象，将第一个参数
                        //作为值保存到这个cookie，并写道客户端，创建身份验证票
                        //第二个参数，表示这个cookie是临时还是永久
                        //自动跳转到你之前访问的页面，如果直接访问的是登录页
                        //则跳转到web.config中配置的defaulturl页面
                         System.Web.Security.FormsAuthentication
                            .RedirectFromLoginPage(man.name, false);
                         return null;
                        //this.User //这个对象就有内容了，就是因为创建了这个验证票
                        //User.Identity.IsAuthenticated 表示验证是否通过
                        //User.Identity.Name 写入在验证票里的值

                    }
                    else
                    {
                        Response.Write("<script>alert('用户名或密码错');</script>");
                        return View();
                    }

                }
            }
            else {
                return View();
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit() {
            //退出
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }
    }
}
