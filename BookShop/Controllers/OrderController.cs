using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Services;
using BookShop.Models.Entities;
using BookShop.Areas.Admin.Models;


namespace BookShop.Controllers
{
    /// <summary>
    /// 下订单相关控制器
    /// </summary>
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        private UserService userService = new UserService();
        private ItemService itemService = new ItemService();
        private OrderService orderService = new OrderService();
        private BookService bookService = new BookService();
        private CategoryService categoryService = new CategoryService();
        private PublisherService publisherService = new PublisherService();
        private IntegralGoodService integralGoodService = new IntegralGoodService();
        private BookShopPlusEntities db = new BookShopPlusEntities ();

        /*

        /// <summary>
        /// 显示当前登录会员的所有订单
        /// 表格：订单编号，下订单日期，订单总价，快递费用，收件人，收件人地址，电话
        /// 操作
        /// [取消订单]注意：只有订单状态为1，未处理的订单可以
        /// 按照日期排序
        /// 订单编号是超链接，点进去看订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            userService.CheckUser();

            //当前登录用户所有订单
            var orderList=orderService.GetOrderByUserId(userService.GetUserIdByLoginId(Convert.ToString(Session["name"])));
            ViewBag.OrderList = orderList;

            if(id!=null)
            {
                int ret=orderService.CompleteOrder(Convert.ToInt32(id));
                if (ret == 0)
                {
                    Response.Write("<script>alert('修改成功!');location.href='/Order/Index';</script>");                   
                }
                else
                {
                    Response.Write("<script>alert('修改失败!');location.href='/Order/Index';</script>");                  
                }
            }               
            return View();
        }



        public ActionResult Index2(int? id)
        {
            userService.CheckUser();

            //当前登录用户所有订单
            var orderList = orderService.GetOrderByUserId(userService.GetUserIdByLoginId(Convert.ToString(Session["name"])));
            ViewBag.OrderList = orderList;

            if (id != null)
            {
                int ret = orderService.CompleteOrder2(Convert.ToInt32(id));
                if (ret == 0)
                {
                    Response.Write("<script>alert('修改成功!');location.href='/Order/Index';</script>");
                }
                else
                {
                    Response.Write("<script>alert('修改失败!');location.href='/Order/Index';</script>");
                }
            }
            return View();
        }



        /// <summary>
        /// 看订单详情,界面同下订单界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            userService.CheckUser();

            //根据订单id查询详情
            var orderBookList = orderService.GetOrderBookByOrderId(id);

            return View(orderBookList);
        }
        */


        /// <summary>
        /// 积分换购商品页面转来的请求
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public ActionResult Add(int goodsId=-1)
        {
            userService.CheckUser();

            //如果有传参进来，获得积分商品对象
            if (goodsId >= 0)
            {
                var good = db.IntegralGoods.Find(goodsId);
                ViewBag.Good = good;
            }           

            //获得当前登录账号
            string name = Session["name"].ToString();
           
            //传递收货地址
            //var orderAddress = userService.GetOrderAddressByUserId(userService.GetUserIdByLoginId(name));

            int userid = userService.GetUserId(Session["name"].ToString());
            var orderAddress = userService.GetOrderAddressByUserId(userid);
            
            ViewBag.OrderAddress = orderAddress;

            return View();
        }


        /// <summary>
        /// 接收数据并下订单
        /// </summary>
        /// <param name="name">收货人姓名</param>
        /// <param name="phone">收件人电话</param>
        /// <param name="address">收件人地址</param>
        /// <param name="remark">订单备注</param>
        /// <param name="sendCash">快递费</param>
        /// <param name="totalu">订单总价</param>
        /// <param name="code">验证码</param>
        /// <param name="integralGoodId">积分商品编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(string name,string phone,string address,string remark,
            int sendCash,decimal totalu,string code,int integralGoodId=-1)
        {
            userService.CheckUser();


            //得到临时字典里保存的验证码，这是在/tools/createImg里保存的
            string code2 = Convert.ToString(TempData["code"]);
            if (code.ToLower() != code2.ToLower())
            {
                Response.Write("<script>alert('验证码输入错误!');window.location.href='/order/add?goodsId=" + integralGoodId + "'</script>");
                Response.End();
                return null;
                //return RedirectToAction("add", new { goodsId = integralGoodId });
            }


            //开始构造订单表
            BookShop.Areas.Admin.Models.Orders order = new Orders();                                          
            order.phone = phone;
            order.address = address;
            order.OrderRemark = remark;
            order.personName = name;
            order.OrderDate = System.DateTime.Now;
            order.flag = 1;
            order.sendCash = sendCash;
            order.TotalPrice = totalu;
            order.UserId = userService.GetUserId(Session["name"].ToString());



            //开始构造订单明细,订购明细数据在购物车中
            order.OrderBook =new List<BookShop.Areas.Admin.Models.OrderBook>();
            foreach(Item item in itemService.Items )
            {
                BookShop.Areas.Admin.Models.OrderBook book = new Areas.Admin.Models.OrderBook();
                book.BookID = item.Id;
                book.Quantity = item.Qty;
                book.UnitPrice = item.UnitPrice;
                order.OrderBook.Add(book);
            }



            //开始构造积分订单明细
            order.OrderIntegral = new List<BookShop.Areas.Admin.Models.OrderIntegral>();
            //先根据订单积分商品编号取出对应积分商品对象
            var goods = db.IntegralGoods.Find(integralGoodId);
            //实例化积分订单表对象
            BookShop.Areas.Admin.Models.OrderIntegral i = new Areas.Admin.Models.OrderIntegral();
            //赋值对应数据
            i.IntegralGoodsID = goods.Id;
            i.Quantity = 1;
            i.PaymentIntegral = goods.PaymentIntegral;
            order.OrderIntegral.Add(i);




            try
            {
                order.Id= orderService.Add(order);//添加订单
                //调用名字为success的视图，并将order对象作为强类型传入
                //成功下订单后，购物车清空
                itemService.RemoveAll();
                //并设置存放购物车的cookie过期时间为现在
                Request.Cookies["_BookShop_Car"].Expires = System.DateTime.Now;
                return View("success",order);
            }
            catch(Exception ex)
            {
                string js = "<script>alert('该订单添加失败!');history.go(-1);</script>";
                return Content(js);
            }
        }



        
        /// <summary>
        /// 秒杀清仓活动下订单
        /// </summary>
        /// <param name="bookid">要买的书编号</param>
        /// <param name="price">当次活动价格</param>
        /// <param name="qty">购买数量</param>
        /// <param name="flag">1-通过秒杀活动进来，2-通过清仓活动进来</param>
        /// <returns></returns>
        public ActionResult SpecialOrder(int bookid, decimal price,int qty,int flag)
        {
            userService.CheckUser();

            //查找图书信息
            var book = db.Books.Find(bookid);
            ViewBag.Book = book;

            //获得当前登录账号
            string name = Session["name"].ToString();

            //传递收货地址
            //var orderAddress = userService.GetOrderAddressByUserId(userService.GetUserIdByLoginId(name));

            int userid = userService.GetUserId(Session["name"].ToString());
            var orderAddress = userService.GetOrderAddressByUserId(userid);

            ViewBag.OrderAddress = orderAddress;

            return View();                               
        }




        [HttpPost]
        public ActionResult SpecialOrder(string name, string phone, string address, string remark,
            int sendCash, decimal totalu, string code, int bookid, decimal price, int qty,int flag)
        {
            userService.CheckUser();


            //得到临时字典里保存的验证码，这是在/tools/createImg里保存的
            string code2 = Convert.ToString(TempData["code"]);
            if (code.ToLower() != code2.ToLower())
            {
                Response.Write("<script>alert('验证码输入错误!');window.location.href='/order/SpecialOrder?bookid=" + bookid + "&price="+price+"&qty="+qty+"&flag="+flag+"'</script>");
                Response.End();
                return null;
                //return RedirectToAction("add", new { goodsId = integralGoodId });
            }


            //开始构造订单表
            BookShop.Areas.Admin.Models.Orders order = new Orders();
            order.phone = phone;
            order.address = address;
            order.OrderRemark = remark;
            order.personName = name;
            order.OrderDate = System.DateTime.Now;
            order.flag = 1;
            order.sendCash = sendCash;
            order.TotalPrice = totalu;
            order.UserId = userService.GetUserId(Session["name"].ToString());


            //开始构造特殊订单
            order.OrderBook = new List<BookShop.Areas.Admin.Models.OrderBook>();
            var book = new BookShop.Areas.Admin.Models.OrderBook();
            book.BookID = bookid;
            book.Quantity = qty;
            book.UnitPrice = price;
            book.flag = flag;
            order.OrderBook.Add(book);

            try
            {
                order.Id = orderService.SpecialAdd(order);//添加订单
                //调用名字为success的视图，并将order对象作为强类型传入
                //成功下订单后，购物车清空
               
                return View("success", order);
            }
            catch (Exception ex)
            {
                string js = "<script>alert('该订单添加失败!');history.go(-1);</script>";
                return Content(js);
            }
        }
         




    }
}
