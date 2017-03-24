using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;

namespace BookShop.Areas.Admin.Controllers
{
    public class BookManage2Controller : Controller
    {
        //
        // GET: /Admin/BookManage2/

        public ActionResult Index(string title,string author,
            int pid=-1,int categoryid=-1,int pageindex=1)
        {
            BookShopPlusEntities db = new BookShopPlusEntities();
            //关闭状态管理，这里只是查询，不需要ef状态跟踪，能够提高点性能

            db.Configuration.AutoDetectChangesEnabled = false;

            var categories = db.Categories.ToList();
            categories.Insert(0, new Categories { Id=-1,Name="--请选择--" });
            SelectList selectCategories = new SelectList(categories, "Id", "Name");

            var publishers = db.Publishers.ToList();
            publishers.Insert(0, new Publishers { Id = -1, Name = "--请选择--" });
            SelectList selectPublishers = new SelectList(publishers, "Id", "Name");

            ViewBag.Categories = selectCategories;
            ViewBag.Publishers = selectPublishers;
            //============================
            //构造where查询条件
            System.Text.StringBuilder where = new System.Text.StringBuilder(" where  1= 1 ");
            if (string.IsNullOrEmpty(title) == false) {
                where.Append(string.Format(" and title like '%{0}%'", title));
            }
            if (string.IsNullOrEmpty(author) == false)
            {
                where.Append(string.Format(" and author like '%{0}%'", author));
            }
            if (categoryid != -1) {
                where.Append(" and categoryid=" + categoryid);
            }
            if (pid != -1)
            {
                where.Append(" and publisherid=" + pid);
            }
            string sql = "select * from books  " + where.ToString();

            int pagesize = 15;
            int skip = (pageindex - 1) * pagesize;
            //必须要先排序
            var books = db.Books.SqlQuery(sql).OrderBy(p=>p.Id)
                 .Skip(skip).Take(pagesize);
            int recordCount = db.Books.SqlQuery(sql).Count();//得到图书总数
            //求总页数
            int pagecount = recordCount % pagesize == 0 ? 
                recordCount / pagesize : recordCount / pagesize + 1;
            //将总记录数和总页数传给视图
            ViewBag.RecordCount=recordCount;//总记录数
            ViewBag.PageCount = pagecount;
            //下面，构造，页面中 页码下拉列表数据源
            //说明下，因为数据源需要构造list集合，而且需要用到键值对
            //所以，用Categories这个类，因为里面有Id,Name属性，符合
            //这里的需求，当然，你愿意的话，也可以去设计一个类 
            //含 key,value的属性即可，这里偷懒下，不设计这个类了
            // public class KeyValue{ public object Key{get;set;},public object Value{get;set;}  }
            List<Categories> nums = new List<Categories>();
            for (int i = 1; i <= pagecount; i++) {
                nums.Add(new Categories { Id=i, Name=i.ToString() });
            }
            SelectList selectNums = new SelectList(nums, "Id", "Name");
            ViewBag.Nums = selectNums; //传给视图
            return View(books);
        }

    }
}
