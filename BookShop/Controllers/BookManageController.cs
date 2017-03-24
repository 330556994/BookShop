using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BookShop.Models.Entities;
using BookShop.Models.Services;

namespace BookShop.Controllers
{
    public class BookManageController : Controller
    {
        //
        // GET: /BookManage/

        public ActionResult Index()
        {
            BookService service = new BookService();
            var list = service.GetList();
            ViewBag.Books = list;//传给视图
            return View();
        }
        public ActionResult Detail(int id) {
            BookService service = new BookService();
            Book book = service.GetSingle(id);//得到单本书
            ViewBag.Book = book;
            return View();
        }

    }
}
