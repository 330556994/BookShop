using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//实体类命名空间
using BookShop.Models.Entities;
//业务类命名空间
using BookShop.Models.Services;

namespace BookShop.Controllers
{
    public class PublisherManageController : Controller
    {
        //
        // GET: /PublisherManage/

        public ActionResult Index(int pageindex=1)
        {

            //<add key="pagesize" value="10"/> 读取配置值
            int pagesize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["pagesize"]);//每页多少条记录

            int start = (pageindex - 1) * pagesize + 1;//起始索引
            int end = pagesize * pageindex;//结束索引
            //实例化业务类
            PublisherService service = new PublisherService();
            //定义集合，并调用业务方法返回出版社集合
            List<Publisher> list = service.GetList(start,end);
            ViewBag.Publishers = list;//传给视图

            //求出总记录数
            int recordCount = service.GetRecordCount();//获得该类别下图书总数量
            //求总页数
            int pageCount = (recordCount % pagesize == 0)
                ? recordCount / pagesize :
                recordCount / pagesize + 1;

            ViewBag.PageCount = pageCount;
            ViewBag.RecordCount = recordCount;


            return View();//调用视图
        }
        /// <summary>
        /// 显示单条记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail() {
            // detail?id=35  url传参 如果多个detail?id=35&name=abc
            //获得url传参中id的值 QueryString是键值对
            //Request是请求对象，封装了客户端的请求
            int id = Convert.ToInt32(Request.QueryString["id"]);
            PublisherService service = new PublisherService();
            Publisher p = service.GetSingle(id);//获得该出版社
            ViewBag.Publisher = p;//传给视图
            return View();
        
        }
        /// <summary>
        /// 删除
        /// action带参数，这个叫映射，自动将 get或post 的键值自动映射
        ///  delete?id=1   <input name="id"/>
        /// <form  method="post"></form>
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(int id) {
            PublisherService service = new PublisherService();
            string js;
            try
            {
                service.Delete(id);//删除
                js = "<script>alert('删除成功');"
                    +"location.href='/PublisherManage/index';</script>";


            }
            catch (Exception ex) {
                js = "<script>alert('删除失败');"
                       + "location.href='/PublisherManage/index';</script>";
            }
            //将一段html或js脚本发送给客户端
            return Content(js);
        }

    }
}
