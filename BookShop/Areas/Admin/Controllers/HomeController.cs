using Accp.Tools;
using BookShop.Areas.Admin.Models;
using BookShop.Models.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BookShop.Models;
using Microsoft.Office.Interop.Excel;

namespace BookShop.Areas.Admin.Controllers
{
    /// <summary>
    /// 作者：四人帮
    /// 时间：2016/12/7
    /// 描述：后台大部分功能实现所依赖的方法都在此控制器中 
    /// </summary>
    public class HomeController : Controller
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ajax调用，返回图书列表
        /// </summary>
        /// <returns></returns>
        [MyValidateForCheckAdmin]
        public ActionResult ListForBook(int page, int rows, Books books)
        {
            //目前这个方法查询功能很有问题
            int skip = (page - 1) * rows;
            string json = "";
            int count = 0;
            var list = db.Books.OrderByDescending(p => p.Id)
                        .Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(),isIntegralGoods=p.IntegralGoods.Count() })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
                count = db.Books.Count();//得到总记录
                if (books.Id != 0)
            {      //输入图书编号查询时
                list = db.Books.Where(p => p.Id == books.Id).OrderByDescending(p => p.Id).Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(), isIntegralGoods = p.IntegralGoods.Count() }).Skip(skip).Take(rows).ToList();
                count = db.Books.Where(p => p.Id == books.Id).Count(); 
            }
                if (!string.IsNullOrEmpty(books.Title))
            {
                list = db.Books.Where(p => p.Title.Contains(books.Title)).OrderByDescending(p => p.Id).Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(), isIntegralGoods = p.IntegralGoods.Count() }).Skip(skip).Take(rows).ToList();
                count = db.Books.Where(p => p.Title.Contains(books.Title)).Count(); 
            }
            if (!string.IsNullOrEmpty(books.Author))
            {
                list = db.Books.Where(p => p.Title.Contains(books.Author)).OrderByDescending(p => p.Id).Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(), isIntegralGoods = p.IntegralGoods.Count() }).Skip(skip).Take(rows).ToList();
                count = db.Books.Where(p => p.Title.Contains(books.Author)).Count(); 
            }
            if (books.PublisherId != 0)
            {
                list = db.Books.Where(p => p.PublisherId == books.PublisherId).OrderByDescending(p => p.Id).Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(), isIntegralGoods = p.IntegralGoods.Count() }).Skip(skip).Take(rows).ToList();
                count = db.Books.Where(p => p.PublisherId == books.PublisherId).Count(); 
            }
            if (books.CategoryId != 0)
            {
                list = db.Books.Where(p => p.CategoryId == books.CategoryId).OrderByDescending(p => p.Id).Select(p => new { p.Id, p.Title, p.Author, Pname = p.Publishers.Name, p.PublishDate, p.ISBN, p.MarketPrice, p.UnitPrice, p.Clicks, p.Flag, p.ClearPrice, Cname = p.Categories.Name, p.TOC, p.ContentDescription, isSeckillGoogs = p.Seckill.Count(), isIntegralGoods = p.IntegralGoods.Count() }).Skip(skip).Take(rows).ToList();
                count = db.Books.Where(p => p.CategoryId == books.CategoryId).Count(); 
            }
            json = LitJson.JsonMapper.ToJson(list);
            
            //使用第三方类库，将对象转换成json格式字符串
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回图书分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForCategory(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.Categories.OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Name, p.SortNum, p.Pid,categoryBookCount=p.Books.Count() })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Categories.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回出版社列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForPublisher(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.Publishers.OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Name,publisherBookCount=p.Books.Count() })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Publishers.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回会员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForUsers(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.Users.OrderBy(p => p.Id)
                        .Select(p => new { p.Id,p.Address,p.Birthday,collectionsCount=p.Collections.Count(),p.LoginId,p.LoginPwd,p.Mail,p.Name,orderCount=p.Orders.Count(),p.Phone,commentCount=p.ReaderComments.Count(),p.RegisterTime,p.scoreCurrent,p.scoreTotal,p.UserRoleId,p.IsFrozen })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Users.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回会员评论列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForReaderComment(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.ReaderComments.OrderByDescending(p => p.Id)
                        .Select(p => new { p.Id, p.Comment, p.Title, Btitle = p.Books.Title, p.Date, Uname = p.Users.Name, p.Flag })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.ReaderComments.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回正在处于秒杀活动中的图书
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForSeckillBooks(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.Seckill.OrderByDescending(p => p.Id)
                        .Select(p => new { p.Id, p.GoodsQty, p.BeginTime, p.BookId, booktitle = p.Books.Title, p.EndTime, p.sysman.name, p.SeckillPrice, UnitPrice = p.Books.UnitPrice, bookimg = p.Books.ISBN, p.Flag })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Seckill.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回正在处于清仓活动中的图书
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForClearBooks(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.Books.Where(p => p.Flag == 1).OrderByDescending(p => p.Id)
                        .Select(p => new { p.Id, p.Title, p.ISBN, p.ClearPrice, p.Author, p.UnitPrice })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Books.Where(p => p.Flag == 1).Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，返回传入参数 flag 类型下的所有订单  和传入的orderid 返回的查询结果
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForOrderByFlag(int page, int rows,int orderid, int flag = 0)
        {
            int skip = (page - 1) * rows;
            string retJson = "";
            if (orderid == 0)
            {
                if (flag == 0)
                {
                    var list = db.Orders.OrderByDescending(p => p.OrderDate)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
                else
                {
                    var list = db.Orders.Where(p => p.flag == flag).OrderByDescending(p => p.Id)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.flag == flag).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
            }
            else {
                if (flag == 0)
                {
                    var list = db.Orders.Where(p=>p.Id==orderid).OrderByDescending(p => p.OrderDate)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.Id == orderid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
                else
                {
                    var list = db.Orders.Where(p => p.flag == flag).Where(p => p.Id == orderid).OrderByDescending(p => p.Id)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.flag == flag).Where(p => p.Id == orderid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
            }
            
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用 返回传入参数订单号下的所有订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForOrderBooksByOrderId(int page, int rows, int orderid=0)
        {
            int skip = (page - 1) * rows;
            string retJson = "";
            var list = db.OrderBook.Where(p => p.OrderID == orderid).OrderByDescending(p => p.Orders.OrderDate)
                    .Select(p => new { p.Id, p.OrderID, p.Quantity, title = p.Books.Title, marketPrice = p.Books.MarketPrice, bookImg = p.Books.ISBN, p.UnitPrice, flagfororders = p.Orders.flag, p.BookID,p.flag })
                    .Skip(skip)
                    .Take(rows)
                    .ToList();
            int count = db.OrderBook.Where(p => p.OrderID == orderid).Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }
        
        /// <summary>
        /// ajax调用，返回所有积分兑换商品
        /// </summary>
        /// <returns></returns>
        public ActionResult ListForIntegralGoods(int page, int rows)
        {
            int skip = (page - 1) * rows;
            var list = db.IntegralGoods.OrderByDescending(p => p.Id)
                        .Select(p => new { p.Id,booktitle=p.Books.Title,p.PaymentIntegral,sectionName=p.IntegralSection.Name,p.BookId,bookimg=p.Books.ISBN})
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.IntegralGoods.Count();//得到总记录
            string json = LitJson.JsonMapper.ToJson(list);
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
            return Content(retJson);
        }

        /// <summary>
        /// ajax调用，根据图书 id 删除图书
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteBookById(int id) {
            string json = "";
            try {
                Books books = db.Books.Find(id);
                db.Books.Remove(books);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，根据类别 id 删除类别
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteCategoryById(int id)
        {
            string json = "";
            try
            {
                Categories categories = db.Categories.Find(id);
                db.Categories.Remove(categories);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，根据出版社 id 删除出版社信息
        /// </summary>
        /// <returns></returns>
        public ActionResult DeletePublisherById(int id)
        {
            string json = "";
            try
            {
                Publishers publishers = db.Publishers.Find(id);
                db.Publishers.Remove(publishers);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，添加图书和修改图书操作
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]//这个特性是指，取消安全检测功能   //默认情况下,提交的数据里有<>是不可以的
        // 在这个方法中传递的参数中  toc 这个参数 带有 html 标签。↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
        public ActionResult AddNewBookMessageOrUpdateBookMessageByAjax(Books books)
        {
            string json = "";
            //先判断是新增还是更新
            if (books.Id==0)
            {   //ajax没有传 bookid 的参数说明是新增操作
                try
                {
                    db.Books.Add(books);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }   
            else {      //执行更新操作
                try {
                    db.Entry(books).State = EntityState.Modified;
                    db.SaveChanges();       
                    json = "{\"success\":2}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":3}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，返回系统菜单中各项的对应的记录数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllMenuCount() {
            int bookCount = db.Books.Count();
            int categoryCount = db.Categories.Count();
            int publishCount = db.Publishers.Count();
            int integralGoodCount = db.IntegralGoods.Count();
            int userCount = db.Users.Count();
            int readerComment = db.ReaderComments.Count();
            int ordersCount = db.Orders.Count();
            int outOrdersCount=db.Orders.Where(p=>p.flag==2).Count();
            int cancelOrdersCount = db.Orders.Where(p => p.flag == 4).Count();
            int returnGoodsOrdersCount = db.Orders.Where(p => p.flag == 9).Count();
            int seckillOrdersCount = db.Seckill.Count();
            int clearOrdersCount = db.Books.Where(p => p.Flag == 1).Count();
            return Content("{\"bookCount\":" + bookCount +
                           ",\"categoryCount\":" + categoryCount +
                           ",\"publishCount\":" + publishCount +
                           ",\"integralGoodCount\":" + integralGoodCount +
                           ",\"userCount\":" + userCount +
                           ",\"readerComment\":" + readerComment +
                           ",\"ordersCount\":" + ordersCount +
                           ",\"outOrdersCount\":" + outOrdersCount +
                           ",\"cancelOrdersCount\":" + cancelOrdersCount +
                           ",\"returnGoodsOrdersCount\":" + returnGoodsOrdersCount +
                           ",\"seckillOrdersCount\":" + seckillOrdersCount +
                           ",\"clearOrdersCount\":" + clearOrdersCount + "}");
        }

        /// <summary>
        /// ajax调用，添加图书类别和修改图书类别操作
        /// <returns></returns>
        public ActionResult AddNewCategoryMessageOrUpdateCategoryMessageByAjax(Categories c)
        {
            string json = "";
            if (c.Id==0)
            {      
                Categories categories = new Categories() {Name=c.Name };
                try
                {
                    db.Categories.Add(categories);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {      //执行更新操作
                try
                {
                    Categories categories = db.Categories.Find(c.Id);
                    categories.Name = c.Name;
                    db.SaveChanges();      
                    json = "{\"success\":2}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":3}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，添加图书出版社和修改图书出版社操作
        /// <returns></returns>
        public ActionResult AddNewPublisherMessageOrUpdatePublisherMessageByAjax(Publishers p)
        {
            string json = "";
            if (p.Id == 0)
            {
                Publishers publishers = new Publishers();
                publishers.Name = p.Name;
                try
                {
                    db.Publishers.Add(publishers);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {      //执行更新操作
                try
                {
                    Publishers publishers = db.Publishers.Find(p.Id);
                    publishers.Name = p.Name;
                    db.SaveChanges();       
                    json = "{\"success\":2}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":3}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，添加积分商品和修改积分商品信息
        /// <returns></returns>
        public ActionResult AddNewIntegralGoodsMessaegeOrUpdateIntegralGoodsMessaegeByAjax(IntegralGoods i)
        {
            string json = "";
            if (i.Id == 0)
            {
                try
                {
                    db.IntegralGoods.Add(i);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {      //执行更新操作
                try
                {
                    db.Entry(i).State = EntityState.Modified;
                    db.SaveChanges();       
                    json = "{\"success\":2}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":3}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，获取所有图书出版社的信息 并以 Json 的格式返回  该方法在添加/修改图书的模态窗口的出版社下拉框中使用过
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPublisherForComboBox()
        {
            return Json(new PublisherService().GetList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ajax调用，获取积分商品的区段信息 并以 Json 的格式返回  该方法在添加积分商品的模态窗体的积分段下拉框中使用过
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIntegralSectionForComboBox()
        {
            return Json(db.IntegralSection.Select(p => new { p.Id,p.Name}).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ajax调用，将管理员选中的行的图书加入到 清仓商品中
        /// </summary>
        /// 商品表增加状态 是否清仓，加清仓价   后台多个功能，能够将普通商品移动到清仓里。
        /// --为了实现清仓功能，在books表中增加 flag 列：0 表示 非清仓商品；  1 表示 清仓商品
        public ActionResult AddBooksToClearGoods(int bookid)
        {
            string json = "";
            //现获取书
            Books books = db.Books.Find(bookid);
            //判断 flag 的值  如果已经为 1 则直接 return-2
            if (books.Flag == 1)
            {
                json = "{\"success\":-2}";  // ajax 获取显示不能重复添加
            }
            else 
            {
                try
                {
                    //此时以满足条件 可以更改 flag 的值了
                    books.Flag = 1;
                    db.SaveChanges();   //执行保存
                    json = "{\"success\":0}";
                }
                catch (Exception ex) {
                    json = "{\"success\":-1}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，将管理员选中的行的图书加入到 秒杀列表中
        /// </summary>
        /// 秒杀表的 flag : --活动状态   规则是 0：未开始；1：已开始；2：已结束
        public ActionResult AddBooksToSeckillGoodsOrUpdateSeckillMessageByAjax(int bookid, string begintime, string endtime, string seckillprice, int sysmanid, int flag, int goodsqty, int id = 0){    //这里后来又加了一个带默认值的 id 参数
            string json = "";
            //由于添加和修改秒杀活动信息两个功能共用一个模态窗口 而且这两个操作也共用一个 onclick 事件 所以可以把 添加和修改放在一个方法中
            //可以根据 ajax 传入参数中是否有  id（秒杀活动的编号）  如果传参的值中 id 为0 则可以判断 ajax 请求的是添加操作  如果不为 0 则可以判断是修改操作  
            //首先判断是新增还是编辑操作
            if (id == 0)
            {  //新增
                //开始判断 秒杀活动中是否已经有 传入的bookid的这本图书 
                Seckill s = db.Seckill.Where(p => p.BookId == bookid).FirstOrDefault();
                if (s != null)      //秒杀活动中已存在
                {
                    json = "{\"success\":-2}";      //直接返回-2  表示不能重复添加
                }
                else
                {
                    try
                    {
                        //这里实例化一个实体类 seckill 并赋上 ajax 传入的参数 然后添加到 seckill 中 
                        Seckill seckill = new Seckill();
                        seckill.BookId = bookid;
                        seckill.BeginTime = Convert.ToDateTime(begintime);
                        seckill.EndTime = Convert.ToDateTime(endtime);
                        seckill.SeckillPrice = Convert.ToDecimal(seckillprice);
                        seckill.SysmanId = sysmanid;
                        seckill.Flag = flag;
                        seckill.GoodsQty = goodsqty;
                        //执行添加操作
                        db.Seckill.Add(seckill);
                        db.SaveChanges();
                        json = "{\"success\":0}";
                    }
                    catch (Exception ex)
                    {
                        json = "{\"success\":-1}";
                    }
                }
            }
            else if(id!=0) { 
                // 开始执行修改操作
                try
                {
                    Seckill seckill = db.Seckill.Find(id);  //获取要修改的 秒杀对象
                    //赋值操作
                    seckill.BookId = bookid;
                    seckill.BeginTime = Convert.ToDateTime(begintime);
                    seckill.EndTime = Convert.ToDateTime(endtime);
                    seckill.SeckillPrice = Convert.ToDecimal(seckillprice);
                    seckill.SysmanId = sysmanid;
                    seckill.Flag = flag;
                    seckill.GoodsQty = goodsqty;
                    db.SaveChanges();           //保存实体中收到得更改
                    json = "{\"success\":0}";
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，修改会员评论星级
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateUserCommentStarByAjax(int commentid, int flag)
        {
            string json = "";
            int userid = db.ReaderComments.Where(p => p.Id == commentid).FirstOrDefault().UserId;   //获取 commentid 这条评论所属的会员
            try
            {
                using (SqlConnection conn = new SqlConnection(DbSqlHelper.connstr))
                {
                    conn.Open();//打开链接
                    SqlCommand cmd = conn.CreateCommand();//创建命令
                    //创建输入参数
                    SqlParameter cid = new SqlParameter("@id", commentid);
                    cid.SqlDbType = SqlDbType.Int;//设置参数类型
                    
                    SqlParameter uid = new SqlParameter("@userid", userid);
                    uid.SqlDbType = SqlDbType.Int;//设置参数类型

                    SqlParameter flagforcomment = new SqlParameter("@flag", flag);
                    flagforcomment.SqlDbType = SqlDbType.Int;//设置参数类型
                    //按照存储过程定义参数的顺序，添加参数
                    cmd.Parameters.Add(cid);
                    cmd.Parameters.Add(uid);
                    cmd.Parameters.Add(flagforcomment);
                    //设置cmd调用格式为存储过程
                    cmd.CommandType = CommandType.StoredProcedure;
                    //设置调用存储过程名字
                    cmd.CommandText = "proc_completeUpdateCommentStar";
                    //如果不返回结果集，就这样调用
                    cmd.ExecuteNonQuery();
                    json = "{\"success\":0}";
                }
            }  
            catch (Exception ex)
            {
                json = "{\"success\":-1}"; 
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，从清仓商品列表中将图书移除
        /// </summary>
        public ActionResult RemoveBookFromClearGoods(int bookid)
        {
            string json = "";
            try
            {
                Books books = db.Books.Find(bookid);
                books.Flag = 0;     //将 该本图书的 flag 设置为 0  表示为普通图书
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，修改清仓商品的价格
        /// </summary>
        public ActionResult EditClearPriceByAjax(int bookid,string newclearprice)
        {
            string json = "";
            try
            {
                Books books = db.Books.Find(bookid);
                books.ClearPrice = Convert.ToDecimal(newclearprice);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，从秒杀商品列表中将图书移除
        /// </summary>
        public ActionResult RemoveBookFromSeckillGoods(int seckillid)
        {
            string json = "";
            try
            {
                Seckill seckill = db.Seckill.Find(seckillid);
                db.Seckill.Remove(seckill);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，从积分商品列表中将图书移除
        /// </summary>
        public ActionResult RemoveBookFromIntegralGoods(int id)
        {
            string json = "";
            try
            {
                IntegralGoods integralGoods = db.IntegralGoods.Find(id);
                db.IntegralGoods.Remove(integralGoods);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        
        /// <summary>
        /// ajax调用，冻结会员帐号
        /// </summary>
        public ActionResult FrozenUserAccount(int userid)
        {
            string json = "";
            //先判断传入的 userid 会员账号是否已被冻结 如果该帐号的 isfrozen 为 1 则直接返回 1
            Users users = db.Users.Find(userid);
            if (users.IsFrozen == 1)
            {
                json = "{\"success\":1}";   
            }
            else {
                try
                {
                    users.IsFrozen = 1;
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，取消冻结会员帐号
        /// </summary>
        public ActionResult CancelFrozenUserAccount(int userid)
        {
            string json = "";
            //先判断传入的 userid 会员账号是否为冻结状态 如果该帐号的 isfrozen 为 0 则直接返回 1
            Users users = db.Users.Find(userid);
            if (users.IsFrozen == 0)
            {
                json = "{\"success\":1}";
            }
            else
            {
                try
                {
                    users.IsFrozen = 0;
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，将订单状态转换为  处理中状态
        /// </summary>
        public ActionResult UpdateOrderFlagToDoing(int orderid)
        {
            string json = "";
            //先判断传入的 orderid 订单是否处于未处理状态（状态为1） 若并不是处于未处理状态 则直接返回 1 前台提示该订单操作失败！
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 2) {
                json = "{\"success\":2}";       // 此时传入参数订单已是处理中订单 返回2 提示订单已处于处理中状态
            }
            else if (orders.flag != 1)
            {
                json = "{\"success\":1}";
            }
            else
            {   //下面进行修改订单状态为处理中
                try
                {
                    orders.flag = 2;
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员取消订单操作
        /// </summary>
        public ActionResult UpdateOrderFlagToAdminCancel(int orderid)
        {
            string json = "";
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 1||orders.flag==2)       //如果该订单 为未处理和处理中状态 则可以进行修改
            {
                //下面进行修改订单状态为管理员已取消状态
                try
                {
                    orders.flag = 5;       //状态编号 5  为管理员已取消状态
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
                
            }
            else
            {       //直接返回1 显示不可修改
                json = "{\"success\":1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员修改订单为已发货状态
        /// </summary>
        public ActionResult UpdateOrderFlagToOutGoods(int orderid)
        {
            string json = "";
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 2)       //如果该订单 为处理中状态 则可以进行发货操作
            {
                //下面进行修改订单状态为管理员已取消状态
                try
                {
                    orders.flag = 3;       //状态编号 3  为已发货状态
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {       //直接返回1 显示订单状态不为 处理中状态 无法修改为已发货状态
                json = "{\"success\":1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员同意会员取消订单 即将订单状态改为 订单已取消状态  编号：8
        /// </summary>
        public ActionResult UpdateOrderFlagToCanceledOrder(int orderid)
        {
            string json = "";
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 4)       //如果该订单 为会员已取消状态 则可以进行同意同户取消订单
            {
                //下面进行修改订单状态为管理员已取消状态
                try
                {
                    orders.flag = 8;       //状态编号 8  为订单已取消状态
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {       //直接返回1 显示订单状态不为 会员已取消状态 无法修改为订单已取消状态
                json = "{\"success\":1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员同意会员退货请求 即将订单状态改为 管理员确认退货  编号：10
        /// </summary>
        public ActionResult UpdateOrderFlagToReturnGoodsOrder(int orderid)
        {
            string json = "";
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 9)       //如果该订单 为用户申请退货状态 则可以进行同意同户取消订单
            {
                try
                {
                    orders.flag = 10;       //状态编号 10  管理员确认退货
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {
                json = "{\"success\":1}";       //这里直接返回1  因为状态不为 9：用户申请退货状态 前台直接显示无法完成操作
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员确认会员退货商品已收到 即将订单状态改为 我方已收货  编号：11
        /// </summary>
        public ActionResult UpdateOrderFlagToGetReturnGoodsOrder(int orderid)
        {
            string json = "";
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 10)       //如果该订单 为管理员确认退货中 则可以进行我方已收货操作
            {
                try
                {
                    orders.flag = 11;       //状态编号 11  我方已收货
                    db.SaveChanges();
                    json = "{\"success\":0}";       //修改成功
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {
                json = "{\"success\":1}";       //这里直接返回1  因为状态不为 10：管理员确认退货中 前台直接显示无法完成操作
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员执行退货订单完成 即将订单状态改为 退货完成  编号：12
        /// </summary>
        public ActionResult UpdateOrderFlagToSuccessReturnGoodsOrder(int orderid)
        {
            string json = "";
            int userid = new UserService().GetUserId(Session["name"].ToString());
            Orders orders = db.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            if (orders.flag == 11)       //如果该订单 为我方已收货状态 则可以进行退货完成操作
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(DbSqlHelper.connstr))
                    {
                        conn.Open();//打开链接
                        SqlCommand cmd = conn.CreateCommand();//创建命令
                        //创建输入参数
                        
                        SqlParameter uid = new SqlParameter("@userid", userid);
                        uid.SqlDbType = SqlDbType.Int;//设置参数类型

                        SqlParameter oid = new SqlParameter("@orderid", orderid);
                        oid.SqlDbType = SqlDbType.Int;//设置参数类型

                        SqlParameter tprice = new SqlParameter("@totalPrice", orders.TotalPrice);
                        tprice.SqlDbType = SqlDbType.Money;//设置参数类型
                        //按照存储过程定义参数的顺序，添加参数
                        cmd.Parameters.Add(uid);
                        cmd.Parameters.Add(oid);
                        cmd.Parameters.Add(tprice);
                        //设置cmd调用格式为存储过程
                        cmd.CommandType = CommandType.StoredProcedure;
                        //设置调用存储过程名字
                        cmd.CommandText = "proc_completeReturnGoodsOrder";
                        //如果不返回结果集，就这样调用
                        cmd.ExecuteNonQuery();
                        json = "{\"success\":0}";
                    }
                }
                catch (Exception ex)
                {
                    json = "{\"success\":-1}";
                }
            }
            else
            {
                json = "{\"success\":1}";       //这里直接返回1  因为状态不为 11：我方已收货状态 前台直接显示无法完成操作
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，验证管理员登录方法
        /// </summary>
        public ActionResult AdminLogin(sysman sysMan)
        {
            string json = "";
            try
            {
                sysman sys = db.sysman.Where(p=>p.name==sysMan.name).Where(p=>p.pwd==sysMan.pwd).FirstOrDefault();
                if (sys != null)
                {
                    Session["adminname"] = sysMan.name;
                    json = "{\"success\":0}";       //登录成功
                }
                else {
                    json = "{\"success\":-1}";
                }
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，管理员账号安全退出方法
        /// </summary>
        public ActionResult AdminExit(sysman sysMan)
        {
            string json = "";
            try
            {
                Session.Abandon();//销毁当前会话
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        /// <summary>
        /// ajax调用，导出会员表信息Excel
        /// </summary>
        public ActionResult ExportUserMessageByExcel()
        {
            string json = "";
            var list = db.Users.ToList();
            //Application  EXCEL  应用程序
            //Workbook 工作簿
            //Worksheet 工作表
            Application app = new Application();//创建一个 EXCEL 应用程序对象
            //app.Visible = true;     //让这个应用程序可见，调试使用，正常应该不可以
            //创建一个新的工作簿，注意，参数是模板，这里不用模板
            Workbook book = app.Workbooks.Add();
            //显得带这个工作表
            Worksheet sheet1 = book.Sheets.get_Item("sheet1");
            //excel中b7表示 第七行的第B列
            //注意 在Excel集合是从下标1开始
            //下面写表头
            //定义一个范围，选中第2行第2列，就相当于 B2
            Range cell = (Range)sheet1.Cells[2, 2];
            cell.Value = "编号";  //写文字到单元格里
            cell = (Range)sheet1.Cells[2, 3];
            cell.Value = "账号";
            cell = (Range)sheet1.Cells[2, 4];
            cell.Value = "密码";
            cell = (Range)sheet1.Cells[2, 5];
            cell.Value = "邮箱";
            cell = (Range)sheet1.Cells[2, 6];
            cell.Value = "生日";
            cell = (Range)sheet1.Cells[2, 7];
            cell.Value = "地址";
            cell = (Range)sheet1.Cells[2, 8];
            cell.Value = "会员等级";
            cell = (Range)sheet1.Cells[2, 9];
            cell.Value = "注册姓名";
            cell = (Range)sheet1.Cells[2, 10];
            cell.Value = "电话号码";
            cell = (Range)sheet1.Cells[2, 11];
            cell.Value = "注册时间";
            cell = (Range)sheet1.Cells[2, 12];
            cell.Value = "当前积分";
            cell = (Range)sheet1.Cells[2, 13];
            cell.Value = "历史积分";
            cell = (Range)sheet1.Cells[2, 14];
            cell.Value = "会员评论数";
            cell = (Range)sheet1.Cells[2, 15];
            cell.Value = "会员订单数";
            cell = (Range)sheet1.Cells[2, 16];
            cell.Value = "会员收藏数";
            cell = (Range)sheet1.Cells[2, 17];
            cell.Value = "账号状态";
            //下面循环这个集合，把数据依次往下写
            int row = 3;
            foreach (Users u in list)
            {
                cell = (Range)sheet1.Cells[row, 2];
                cell.Value = u.Id;
                cell = (Range)sheet1.Cells[row, 3];
                cell.Value = u.LoginId;
                cell = (Range)sheet1.Cells[row, 4];
                cell.Value = u.LoginPwd;
                cell = (Range)sheet1.Cells[row, 5];
                cell.Value = u.Mail;
                cell = (Range)sheet1.Cells[row, 6];
                cell.Value = u.Birthday;
                cell = (Range)sheet1.Cells[row, 7];
                cell.Value = u.Address;
                cell = (Range)sheet1.Cells[row, 8];
                cell.Value = u.UserRoleId;
                cell = (Range)sheet1.Cells[row, 9];
                cell.Value = u.Name;
                cell = (Range)sheet1.Cells[row, 10];
                cell.Value = u.Phone;
                cell = (Range)sheet1.Cells[row, 11];
                cell.Value = u.RegisterTime;
                cell = (Range)sheet1.Cells[row, 12];
                cell.Value = u.scoreCurrent;
                cell = (Range)sheet1.Cells[row, 13];
                cell.Value = u.scoreTotal;
                cell = (Range)sheet1.Cells[row, 14];
                cell.Value = u.ReaderComments.Count;
                cell = (Range)sheet1.Cells[row, 15];
                cell.Value = u.Orders.Count;
                cell = (Range)sheet1.Cells[row, 16];
                cell.Value = u.Collections.Count;
                cell = (Range)sheet1.Cells[row, 17];
                cell.Value = u.IsFrozen;
                row++;
            }
            //创建一个 文件名
            string file = Tools.FileHelper.CreateFileName() + ".xlsx";
            string html = "";
            //保存文件需要用到物理路径
            string filename = Server.MapPath("~/excelfiles/" + file);
            try
            {
                book.SaveAs(filename);
                json = "{\"success\":0}";

            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            finally
            {
                book.Close();      //不管成功与否工作薄都关掉
                app.Quit();
            }
            return Content(json);

        }

        /// <summary>
        /// ajax调用，新增管理员操作
        /// </summary>
        public ActionResult AdminRegister(sysman sysMan)
        {
            string json = "";
            try
            {
                sysman sys = db.sysman.Where(p => p.name.Equals(sysMan.name)).FirstOrDefault();
                if (sys == null)
                {
                    db.sysman.Add(sysMan);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
                else {
                    json = "{\"success\":1}";      //前台获取之后提示 该名称已被注册！！！
                }
                
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
