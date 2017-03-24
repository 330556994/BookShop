using BookShop.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;

namespace BookShop.Controllers
{
    public class CommonViewController : Controller
    {
        //
        // GET: /CommonView/


        private BookShopPlusEntities db = new BookShopPlusEntities();



        public ActionResult Index()
        {
            return View();
        }





        /// <summary>
        /// 显示图书分类，返回分部视图(内联视图）
        /// 首页，图书详情页也会调用
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowCategory()
        {
            var cate = db.Categories.ToList();
            //return view(obj) 返回整个完整视图
            //这是返回的是分部视图，即该视图只是返回一段html脚本罢了
            return PartialView(cate);//强类型

        }



    }
}
