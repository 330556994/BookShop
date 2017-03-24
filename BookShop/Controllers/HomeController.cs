using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Entities;
using BookShop.Models.Services;
using BookShop.Models;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        [MySqlValidate]
        public ActionResult Index()
        {
          
         //   CategoryService categoryService = new CategoryService();
            PublisherService publisherService = new PublisherService();
            BookService bookService = new BookService();

        //    List<Category> categories = categoryService.GetList();
          //  ViewBag.Categories = categories;

            List<Publisher> publishers = publisherService.GetList();
            ViewBag.Publishers = publishers;

            List<Book> newBooks = bookService.GetNewBooks(12);//获得新书排行榜
            ViewBag.NewBooks = newBooks;

            List<Book> hotBooks = bookService.GetHotBooks(12);//获得热销排行榜
            ViewBag.HotBooks = hotBooks;
            //得到首页推荐图书信息
            List<Book> homeBooks = bookService.GetHomeBooks(18);
            ViewBag.HomeBooks = homeBooks;

            return View();
        }

    }
}
