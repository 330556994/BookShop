using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//后台业务类命名空间
using BookShop.Areas.Admin.Models.Services;
//实体是共用的
using BookShop.Models.Entities;
namespace BookShop.Areas.Admin.Controllers
{
    public class BookManageController : Controller
    {
        //
        // GET: /Admin/BookManage/
        /// <summary>
        /// 图书管理页面
        /// </summary>
        /// <param name="categoryid">查询条件之图书类别编号</param>
        /// <param name="title">图书书名</param>
        /// <param name="pageindex">页索引</param>
        /// <returns></returns>
        public ActionResult Index(int categoryid=0,string title="",int pageindex=1)
        {
            BookService bookservice = new BookService();
            #region 这段代码是用来构造图书类别下拉列表数据源
            CategoryService categoryservice = new CategoryService();
            var categories = categoryservice.GetList();
            //给下拉列表插入一个请选择
            categories.Insert(0, new Category { Id=-1,Name="--请选择图书类别--"  });
            //创建下拉列表需要的数据源
            //参数一categories 集合
            //id 表示，生成的option value的值
            //name 是生存的文本值，这是集合中类的属性
            SelectList selectCategory = new SelectList(categories,"Id","Name");
            ViewBag.Categories = selectCategory;//数据源传给视图
           #endregion
            
            //<add key="pagesize" value="10"/> 读取配置值
            int pagesize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["pagesize"]);//每页多少条记录
            int start = (pageindex - 1) * pagesize + 1;//起始索引
            int end = pagesize * pageindex;//结束索引            
            var list = bookservice.GetList(start,end,categoryid,title);
            return View(list);
        }

    }
}
