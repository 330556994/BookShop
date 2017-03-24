using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShop.Controllers
{
    /// <summary>
    /// 作者：四人帮
    /// 时间：2016/12/7
    /// 描述：该控制器主要为用户返回秒杀页面 秒杀的下订单功能在 orders 控制器中
    /// </summary>
    public class SeckillController : Controller
    {
        /// <summary>
        /// 商城活动页面
        /// </summary>
        /// <returns></returns>
        /// 
        /*
         public ActionResult MallAct() {
             return View();
         }
        */
        /// <summary>
        /// 遍历秒杀图书表  返回秒杀图书信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSecKill()
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            var list = db.Seckill.OrderByDescending(p => p.BeginTime).ToList();
            return View(list);
        }
        ////测试
        //public ActionResult AddSecKill()
        //{
        //    BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
        //    BookShop.Areas.Admin.Models.Seckill secKill = new Areas.Admin.Models.Seckill();
        //    var books = db.Books.Find(5038);
        //    secKill.Books = books;
        //    secKill.sysman = db.sysman.Find(1);
        //    secKill.BeginTime = System.DateTime.Now;
        //    secKill.EndTime = System.DateTime.Now.AddHours(2);
        //    secKill.SeckillPrice = Convert.ToDecimal(Convert.ToInt32(books.UnitPrice) * 0.3);
        //    secKill.Flag = 1;
        //    secKill.GoodsQty = 100;
        //    string json = "";
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Seckill.Add(secKill);
        //            db.SaveChanges();
        //            json = "{\"success\":0}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        json = "{\"success\":-1}";
        //    }
        //    return Content(json);

        //}

        //public ActionResult GetTime()
        //{
        //    BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
        //    BookShop.Areas.Admin.Models.Seckill secKill = db.Seckill.Find(13);
        //    int hour = Convert.ToInt32(secKill.EndTime.Hour) - Convert.ToInt32(secKill.BeginTime.Hour);
        //    int minute = Convert.ToInt32(secKill.EndTime.Minute) - Convert.ToInt32(secKill.BeginTime.Minute);
        //    int realtime = hour * 3600 + minute * 60;
        //    return Content("{\"realtime\":" + realtime + "}");

        //}
        ////验证数据库里的datetime
        //public static int i = 8;
        //public ActionResult Test()
        //{
        //    BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
        //    string sql = string.Format("insert into test2 values({0},'{1}')", i++, System.DateTime.Now);
        //    int ret = db.Database.ExecuteSqlCommand(sql);
        //    return Content("" + ret);
        //}
    }
}
