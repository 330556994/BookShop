using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Services;
using System.Web.Providers.Entities;
using BookShop.Areas.Admin.Models;
using System.Data.SqlClient;
using System.Data;
using Accp.Tools;
namespace BookShop.Controllers
{
    /// <summary>
    /// 作者：四人帮
    /// 时间：2016/12/6
    /// 描述：前台会员大部分功能实现所依赖的方法都在此控制器中 
    /// </summary>
    public class UserController : Controller
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();
        //
        // GET: /User/
        /// <summary>
        /// 会员面板首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            UserService userService = new UserService();
            userService.CheckUser();//登录验证
            ViewBag.SecKill = db.Seckill.OrderByDescending(p => p.BeginTime).ToList();  //将秒杀信息放入viewbag
            return View();
        }

        //登录
        public ActionResult Login()
        {
            return View();
        }

        //登录 post 请求
        [HttpPost]//特性，特性是一种特殊的类型，用来修饰类和方法和属性
        public ActionResult Login(string name, string pwd,
            string code,//验证码
            bool rememberMe = false)
        {
            //得到临时字典里保存的验证码，这是在/tools/createimg
            string code2 = Convert.ToString(TempData["code"]);
            if (code.ToLower() != code2.ToLower())
            {
                //验证码错误
                Response.Write("<script>alert('验证码输入错误');</script>");
                return View();

            }

            UserService userService = new UserService();
            bool ret = userService.Login(name, pwd);
            List<string> users = null; //定义表示在线帐号的集合变量
            if (ret == true)
            {
                //登录成功后，将当前登录帐号保存的application级别变量里
                HttpContext.Application.Lock();
                users = HttpContext.Application["users"]
                    as List<string>;
                users.Add(name);
                HttpContext.Application.UnLock();
                //创建一个session 变量，保存用户名
                Session["name"] = name;
                //判断是要要记住密码，如果是，则创建一个永久cookie，写到客户端去
                if (rememberMe == true)
                {
                    //创建cookie
                    HttpCookie cookie = new HttpCookie("_accp_name");
                    cookie.Value = name;//保存值
                    //设置cookie超时时间
                    cookie.Expires = System.DateTime.Now.AddMonths(3);
                    //将cookie写入客户端
                    Response.Cookies.Add(cookie);
                }


                //获取url传参值
                string returnurl = Request.QueryString["returnurl"];
                if (string.IsNullOrEmpty(returnurl) == true)
                {
                    //跳转到会员面板首页
                    return Redirect("/user/index");
                }
                else
                {
                    return Redirect(returnurl);
                }
            }
            else
            {
                Response.Write("<script>alert('用户名或密码错,或者您的账户已经被冻结！！！');</script>");
                return View();
            }
        }
        /// <summary>
        /// 修改个人密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePwd()
        {
            UserService userService = new UserService();
            userService.CheckUser();//登录验证
            return View();
        }
        /// <summary>
        /// 会员退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            Session.Abandon();//销毁当前会话
            //再次写入一个cookie，设置过期时间，立马失效
            HttpCookie cookie = new HttpCookie("_accp_name");
            //cookie.Value = name;//保存值
            //设置cookie超时时间
            cookie.Expires = System.DateTime.Now.AddMonths(-3);
            //将cookie写入客户端
            Response.Cookies.Add(cookie);
            ////////////////////////////////////////////////////////////////
            // 这里需要处理，会员退出后，把全局users里的在线列表
            //里，删除当前的会员帐号
            ///////////////////////////////////////////////////////////
            return RedirectToAction("login");//跳转到当前控制器的login
        }
        /// <summary>
        /// 会员注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        //修改会员个人信息页面显示
        public ActionResult Edit()
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            Users user = db.Users.Find(userid);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        
        //修改会员个人信息操作
        public ActionResult UpdateUserMessage(Users users) {
            string json = "";
            try {
                Users u = db.Users.Where(p => p.Id == users.Id).FirstOrDefault();
                u.Mail = users.Mail;
                u.LoginId = users.LoginId;
                u.LoginPwd = users.LoginPwd;
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        //获取会员订单信息
        public ActionResult GetOrder(int page, int rows, int orderid = 0, int flag = 0)
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());

            int skip = (page - 1) * rows;
            string retJson = "";
            if (orderid == 0)
            {
                if (flag == 0)
                {
                    var list = db.Orders.Where(p => p.UserId == userid).OrderByDescending(p => p.OrderDate)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.UserId == userid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
                else
                {
                    var list = db.Orders.Where(p => p.flag == flag).Where(p => p.UserId == userid).OrderByDescending(p => p.Id)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.flag == flag).Where(p => p.UserId == userid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
            }
            else
            {
                if (flag == 0)
                {
                    var list = db.Orders.Where(p => p.Id == orderid).Where(p => p.UserId == userid).OrderByDescending(p => p.OrderDate)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.Id == orderid).Where(p => p.UserId == userid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
                else
                {
                    var list = db.Orders.Where(p => p.flag == flag).Where(p => p.UserId == userid).Where(p => p.Id == orderid).OrderByDescending(p => p.Id)
                            .Select(p => new { p.Id, p.OrderDate, loginId = p.Users.LoginId, p.TotalPrice, p.personName, p.phone, p.address, p.sendCash, p.OrderRemark, p.flag })
                            .Skip(skip)
                            .Take(rows)
                            .ToList();
                    int count = db.Orders.Where(p => p.UserId == userid).Where(p => p.flag == flag).Where(p => p.Id == orderid).Count();//得到总记录
                    string json = LitJson.JsonMapper.ToJson(list);
                    retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}", "{", count, json, "}");
                }
            }

            return Content(retJson);
        }

        //获取会员评论信息
        public ActionResult GetComment()
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var list = db.ReaderComments.Where(p => p.UserId == userid)
                .Select(p => new { p.Id, p.Comment, p.Title, p.Flag, p.Date, p.Books.ISBN }).OrderByDescending(p => p.Id)
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取会员积分信息
        public ActionResult GetCurrentAndTotalScore()
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            Users users = db.Users.Find(userid);
            string json = "";
            if (users != null)
            {
                json = "{\"currentScore\":" + users.scoreCurrent + ",\"totalScore\":" + users.scoreTotal + "}";
            }
            return Content(json);
        }

        //获取会员收藏信息  
        public ActionResult GetCollection()
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var list = db.Collections
                .Select(p => new { p.Id, booktitle = p.Books.Title, p.Books.ISBN, p.CollectTime }).OrderByDescending(p => p.Id)
                .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取会员签到信息
        public ActionResult ResiRegistration_list()
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var list = db.Registration.Where(p => p.UserId == userid)
                .Select(p => new { p.Id, p.Content, p.RegistrationTime, p.Flag }).OrderByDescending(p => p.Id)
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //获取积分历史信息
        public ActionResult ShowScoreHistory()
        {
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var list = db.ScoreHistory.Where(p => p.UserId == userid).Select(p => new { p.Id, p.SingleScore, p.CreateTime, p.Descriptions }).OrderByDescending(p => p.CreateTime).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //删除会员评论信息
        public ActionResult DeleteComment(int commentid)
        {
            string json = "";
            try
            {
                ReaderComments comments = db.ReaderComments.Find(commentid);
                db.ReaderComments.Remove(comments);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        //删除会员收藏信息
        public ActionResult DeleteCollections(int collectionsid)
        {
            string json = "";
            try
            {
                Collections collections = db.Collections.Find(collectionsid);
                db.Collections.Remove(collections);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        //执行会员签到操作
        public ActionResult UserRegistration(string content)
        {
            int userid = new UserService().GetUserId(Session["name"].ToString());   //先获取当前  当前登陆用户的 id
            string json = "";
            try
            {
                //这里调用 存储过程--EXEC proc_completeUserRegistration 3,'签到内容' --执行存储过程测试	
                using (SqlConnection conn = new SqlConnection(DbSqlHelper.connstr))
                {
                    conn.Open();//打开链接
                    SqlCommand cmd = conn.CreateCommand();//创建命令
                    //创建输入参数
                    SqlParameter uid = new SqlParameter("@userid", userid);
                    uid.SqlDbType = SqlDbType.Int;//设置参数类型
                    SqlParameter contents = new SqlParameter("@contents", content);
                    //按照存储过程定义参数的顺序，添加参数
                    cmd.Parameters.Add(uid);
                    cmd.Parameters.Add(contents);
                    //设置cmd调用格式为存储过程
                    cmd.CommandType = CommandType.StoredProcedure;
                    //设置调用存储过程名字
                    cmd.CommandText = "proc_completeUserRegistration";
                    //定义输出参数
                    SqlParameter returnValue = new SqlParameter("@returnValue", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);
                    cmd.ExecuteNonQuery();
                    int ret = Convert.ToInt32(returnValue.Value);
                    if (ret == -1)
                    {    //说明该用户当日已经签到过了
                        json = "{\"success\":1}";
                    }
                    else {
                        json = "{\"success\":0}";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }

        //更改订单状态方法
        public ActionResult UserUpdateOrderFlag(int orderid, int flag)
        {
            string json = "";
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            Orders orders = db.Orders.Find(orderid);
            if (flag == 4)
            {    //此时的状况为 取消订单处理

                if (orders.flag != 1)
                {   //订单状态 1:未处理  只有这个状态会员可以取消
                    json = "{\"success\":1}";
                }
                else
                {
                    try
                    {
                        orders.flag = flag;
                        db.SaveChanges();
                        json = "{\"success\":0}";
                    }
                    catch (Exception ex)
                    {
                        json = "{\"success\":-1}";
                    }
                }
            }
            else if (flag == 9)  //此时的状况为 退货处理
            {
                if (orders.flag != 7)     //订单状态 7.已完成   只有这两个状态会员才可以申请退货
                {
                    json = "{\"success\":1}";
                }
                else
                {
                    try
                    {
                        orders.flag = flag;
                        db.SaveChanges();
                        json = "{\"success\":0}";
                    }
                    catch (Exception ex)
                    {
                        json = "{\"success\":-1}";
                    }
                }
            }
            else if (flag == 6)  //此时的状况为 已收货处理
            {
                if (orders.flag != 3)     //订单状态 3.已发货  只有这个状态会员才可以发出已收货请求
                {
                    json = "{\"success\":1}";
                }
                else
                {
                    try
                    {
                        orders.flag = flag;
                        db.SaveChanges();
                        json = "{\"success\":0}";
                    }
                    catch (Exception ex)
                    {
                        json = "{\"success\":-1}";
                    }
                }
            }
            else if (flag == 7)  //此时的状况为 订单已完成处理
            {
                if (orders.flag != 6)     //订单状态 6.已收货  只有这个状态会员才可以发出完成订单请求
                {
                    json = "{\"success\":1}";
                }
                else
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

                            SqlParameter tPrice = new SqlParameter("@totalPrice",orders.TotalPrice);
                            tPrice.SqlDbType = SqlDbType.Money;//设置参数类型
                            //按照存储过程定义参数的顺序，添加参数
                            cmd.Parameters.Add(uid);
                            cmd.Parameters.Add(oid);
                            cmd.Parameters.Add(tPrice);
                            //设置cmd调用格式为存储过程
                            cmd.CommandType = CommandType.StoredProcedure;
                            //设置调用存储过程名字
                            cmd.CommandText = "proc_completeUserOrder";
                            //如果不返回结果集，就这样调用
                            cmd.ExecuteNonQuery();
                        }
                        json = "{\"success\":0}";
                    }
                    catch (Exception ex)
                    {
                        json = "{\"success\":-1}";
                    }
                }
            }
            return Content(json);
        }

        //会员发表评论方法
        public ActionResult UserComment(int bookid, string content)
        {   //传入参数图书 id
            string json = "";
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            ReaderComments comments = new ReaderComments();
            comments.BookId = bookid;
            comments.UserId = userid;
            comments.Flag = 0;
            comments.Comment = content;
            comments.Title = "这本书写的朕踏马好";
            comments.Date = System.DateTime.Now;
            try
            {
                db.ReaderComments.Add(comments);
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        
        //这个方法获得的是 会员连续 登录的天数  必须是连续登录才能获得  相应的积分
        public ActionResult GetRegistrationCount(int year, int month, int day)
        {
            //先获取当前  当前登陆用户的 id
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var list = db.Registration.Where(p => p.UserId == userid).OrderByDescending(p => p.RegistrationTime).ToList();  //使用倒序  
            string json = "";
            //如果 list 不为空 说明 当前登录会员有过签到 记录 但是是否是连续签到的还不得而知
            if (list.Count() != 0)
            {

                if (list[0].RegistrationTime.Year != year || list[0].RegistrationTime.Month != month + 1 || list[0].RegistrationTime.Day + 1 != day)
                {
                    json = "{\"totalCount\":" + list.Count() + ",\"lianxuCount\":0}";
                }
                //数据表中 flag 字段 的值即为连续签到天数  那么获取最近一次签到的记录的 flag 值 即可获得 连续签到天数  总共签到天数 可以取集合的count
                if (list[0].RegistrationTime.Year == year && list[0].RegistrationTime.Month == month + 1 && list[0].RegistrationTime.Day + 1 == day)
                {
                    json = "{\"totalCount\":" + list.Count() + ",\"lianxuCount\":" + list[0].Flag + "}";
                }
                if (list[0].RegistrationTime.Year == year && list[0].RegistrationTime.Month == month + 1 && list[0].RegistrationTime.Day == day)
                {
                    json = "{\"totalCount\":" + list.Count() + ",\"lianxuCount\":" + list[0].Flag + "}";
                }
            }
            else
            {
                //如果返回 0  则前台 可以直接 给签到天数 +1  并且积分 也只 +1
                json = "{\"success\":0}";
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















