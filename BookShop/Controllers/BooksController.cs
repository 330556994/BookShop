using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Entities;
using BookShop.Models.Services;
namespace BookShop.Controllers
{
    public class BooksController : Controller
    {
        //
        // GET: /Books/
        /// <summary>
        /// 显示图书列表
        /// </summary>
        /// <param name="id">得到传入的图书类别</param>
        /// <param name="pageindex">第几页</param>
        /// <returns></returns>
        public ActionResult Index(int id,int pageindex=1,
            string sort="a.id  desc")
        {
            //<add key="pagesize" value="10"/> 读取配置值
            int pagesize =Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["pagesize"]) ;//每页多少条记录


            int start=(pageindex-1)*pagesize +1;//起始索引
            int end=pagesize*pageindex;//结束索引
            
            
            CategoryService categoryService = new CategoryService();
            BookService bookService = new BookService();
            List<Book> books = bookService.GetList(start,end,sort,id);//GetList(id);//取出该类别下图书

            List<Category> list = categoryService.GetListWithBooCount();
            ViewBag.Categories = list;//传给视图
            //求出总记录数
            int recordCount = bookService.GetRecordCount(id);//获得该类别下图书总数量
            //求总页数
            int pageCount = (recordCount % pagesize == 0) 
                ? recordCount / pagesize :
                recordCount / pagesize + 1;

            ViewBag.PageCount = pageCount;
            ViewBag.RecordCount = recordCount;

            //将集合传给视图，这种传法叫强类型视图            
            return View(books);
        }
        /// <summary>
        /// 显示图书详情
        /// </summary>
        /// <param name="id">图书编号</param>
        /// <returns></returns>
        public ActionResult Detail(int id) {
            BookService bookservice = new BookService();
            Book book = bookservice.GetSingle(id); //获得单本图书
            return View(book);//强类型视图
        }

    }
}
