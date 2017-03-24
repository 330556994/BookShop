using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;

namespace BookShop.Controllers
{
    /// <summary>
    /// 作者:李天页
    /// 功能描述:清仓书籍相关控制器
    /// </summary>
    public class ClearBooksController : Controller
    {
        //
        // GET: /ClearBooks/


        private BookShopPlusEntities db = new BookShopPlusEntities();


        public ActionResult Index( int pageindex = 1, string sort = "0")
        {         
            //传递该类别下有多少本书
            List<int> counts = new List<int>();
            var categories = db.Categories.ToList();
            foreach (Categories c in categories)
            {
                var number = db.Books.Where(p => p.CategoryId == c.Id).Count();
                counts.Add(number);
            }
            ViewBag.Counts = counts;


            //查询所有图书类别
            ViewBag.CategoriesList = categories;


            //查看有多少本书处于清仓状态
            var count = db.Books.Where(p => p.Flag == 1).Count();
            ViewBag.Count = count;

       

            //每页多少条记录
            int pagesize = Convert.ToInt32(db.Config.Where(p => p.KeyField.Equals("pagesize")).ToList()[0].ValueField);
            ViewBag.PageSize = pagesize;
            //跳过多少条
            int skip = (pageindex - 1) * pagesize;
            //取出多少条
            int take = pagesize;

            //书信息
            IEnumerable<Books> books=null;
            switch(sort){
                case "0":
                    books = db.Books.Where(p => p.Flag == 1).OrderBy(p => p.Id).Skip(skip).Take(take);
                    break;
                case "1":
                    books = db.Books.Where(p => p.Flag == 1).OrderByDescending(p => p.Id).Skip(skip).Take(take);
                    break;
                case "2":
                    books = db.Books.Where(p => p.Flag == 1).OrderBy(p => p.ClearPrice).Skip(skip).Take(take);
                    break;
                case "3":
                    books = db.Books.Where(p => p.Flag == 1).OrderByDescending(p => p.ClearPrice).Skip(skip).Take(take);
                    break;
                case "4":
                    books = db.Books.Where(p => p.Flag == 1).OrderBy(p => p.PublishDate).Skip(skip).Take(take);
                    break;
                case "5":
                    books = db.Books.Where(p => p.Flag == 1).OrderByDescending(p => p.PublishDate).Skip(skip).Take(take);                  
                    break;
            }
            
            return View(books);
        }

    }
}
