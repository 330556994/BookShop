using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Entities;
using BookShop.Models.Services;
namespace BookShop.Controllers
{
    public class CarController : Controller
    {
        //
        // GET: /Car/

        public ActionResult Index()
        {
            ItemService service = new ItemService();
            return View(service.Items);
        }
        /// <summary>
        /// 添加到购物车
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Buy(int id) { 
            /*1. 根据id得到图书对象
             * 2.根据图书对象创建购物项对象
             * 3.调用购物车操作类完成添加
             * 4. 返回当前视图，并提示添加成功
             */
            BookService bookService = new BookService();
            Book book = bookService.GetSingle(id);//获得这本书
            //根据图书对象创建购物项对象
            Item item = new Item();
            item.Id = book.Id;
            item.Title = book.Title;
            item.UnitPrice = book.UnitPrice;
            item.MarketPrice = book.MarketPrice;
            item.Qty = 1;
            //将购物项item添加到购物车
            ItemService itemService = new ItemService();
            itemService.Add(item);
            //返回之前访问视图，并提示添加成功
            string js = "<script>alert('购买成功');history.go(-1);</script>";
            return Content(js);
            
        
        }
        /// <summary>
        /// ajax调用，添加到购物车
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult Buy2(int bookid)
        {
            /*1. 根据id得到图书对象
             * 2.根据图书对象创建购物项对象
             * 3.调用购物车操作类完成添加
             * 4. 返回当前视图，并提示添加成功
             */
            BookService bookService = new BookService();
            Book book = bookService.GetSingle(bookid);//获得这本书
            //根据图书对象创建购物项对象
            Item item = new Item();
            item.Id = book.Id;
            item.Title = book.Title;
            item.UnitPrice = book.UnitPrice;
            item.MarketPrice = book.MarketPrice;
            item.Qty = 1;
            //将购物项item添加到购物车
            ItemService itemService = new ItemService();
            itemService.Add(item);
            return PartialView("showcar");
        }

        


        public ActionResult Collection(int bookid)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            BookShop.Areas.Admin.Models.Collections collection = new Areas.Admin.Models.Collections();
            BookService bookService = new BookService();
            Book book = bookService.GetSingle(bookid);//获得这本书
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            collection.UserId = userid;
            collection.BookId = bookid;
            collection.CollectTime = System.DateTime.Now;
            string json = "";
            try
            {
                if (ModelState.IsValid)
                {
                    db.Collections.Add(collection);
                    db.SaveChanges();
                    json = "{\"success\":0}";
                }
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        // 检查 传入用户的收藏中是否 存在传入参数值bookid 的那本书  如果存在则返回json为-1
        public ActionResult CheckCollection(int bookid) {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            BookShop.Areas.Admin.Models.Collections collection = new Areas.Admin.Models.Collections();
            var collect = db.Collections.Where(p =>p.BookId == bookid).Where(p=>p.UserId==userid).FirstOrDefault();
            string json = "";
            if (collect == null)
            {
                json = "{\"success\":0}";
            }
            else {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        
        /// <summary>
        /// 删除购物项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id) { 
          //1. 调用购物车操作类的删除方法
          //2. 提示成功信息，然后回到购物车列表页
            ItemService service = new ItemService();
            string js;
            try
            {
                service.Delete(id);//删除
                js = "<script>alert('删除成功');location.href='/car/index';</script>";
                //return RedirectToAction("index");
            }
            catch (Exception ex) {
                js = "<script>alert('删除失败');location.href='/car/index';</script>";
            }
            return Content(js);
        }
        /// <summary>
        /// 完成更新购买数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public ActionResult Update(int id, int qty) { 
         //思路： 调用购物车操作类的更新方法，更新成功后，直接调转
            //到index action，显示购物车列表
            ItemService itemservice = new ItemService();
            itemservice.Update(id, qty);
            return RedirectToAction("index");
        }

    }
}
