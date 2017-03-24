using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Models.Entities;
using BookShop.Models.Services;
using System.Data.SqlClient;
using Accp.Tools;
using System.Data;

using BookShop.Models;
using Microsoft.Office.Interop.Excel;//导入excel组件库

using System.Reflection;//反射需要的命名空间

namespace BookShop.Controllers
{
    public class DemoController : Controller
    {
        //
        // GET: /Demo/
        /// <summary>
        /// 这是演示布局页
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo1()
        {
            return View();
        }
        /// <summary>
        /// 演示request对象
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo2() {
            return View();
        }
        /// <summary>
        /// 获得get请求的值
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo2_1() {
            string id = Request.QueryString["id"];
            string name = Request.QueryString["name"];
            return Content(id + "   " + name);
        }
        /// <summary>
        /// 得到post请求数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo2_2()
        {
            string name = Request.Form["name"];
            string pwd = Request["pwd"];//先查找querystirng，后查找form，最后查环境变量
            return Content(pwd + "   " + name);
        }
        /// <summary>
        /// 这个案例演示，多值cookie,一个cookie保存多个值
        /// 一般，客户端的cookie会限制数量，所以，如果需要保存多个信息时
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo3() {
            HttpCookie cookie = new HttpCookie("_accp_test");
            //中文写入时先编码
            cookie.Values["name"] = Server.UrlEncode( "张三");
            cookie.Values["time"] = System.DateTime.Now.ToString();
            cookie.Values["ip"] = Request.ServerVariables["LOCAL_ADDR"];
            cookie.Expires = System.DateTime.Now.AddDays(10);
            Response.Cookies.Add(cookie);//写到客户端
            return Content("<a href='/demo/demo3_1'>读取写入的cookie值</a>");
        }
        /// <summary>
        /// 读取多值cookie
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo3_1() {
            HttpCookie cookie = Request.Cookies["_accp_test"];
            string html = string.Format("帐号:{0},时间:{1},IP:{2}"
                , Server.UrlDecode( cookie.Values["name"]) //读取时解码
                , cookie.Values["time"]
                , cookie.Values["ip"]
                );
            return Content(html);
        }
        /// <summary>
        /// 这个案例是用来测试，当前在线会员的。
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo4() {
            return View();
        
        }
        /// <summary>
        /// 演示可空类型
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo5() {
            int a = 5;
            int? b = 10;
            Nullable<int> c=10;
            int num = c ?? 0;//??  如果c为null，则值为0，否则为c
            if (c.HasValue == false)
            {
                return Content("c没有值");
            }
            else {
                return Content("c有值");
            }
        }
        /// <summary>
        /// 路由演示
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo6(int year) {
            //得到路由表中，占位符为year的参数值
            int m_year = Convert.ToInt32(RouteData.Values["year"]);
            int m_month = Convert.ToInt32(RouteData.Values["month"]);
            int m_day = Convert.ToInt32(RouteData.Values["day"]);
            return Content(string.Format("year={0},month={1},day={2}"
                , year, m_month, m_day
                ));

        }
        /// <summary>
        /// 演示url.Action, Html.ActionLink()
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo7() {
            return View();
        }
        /// <summary>
        /// 演示调用存储过程 返回结果集
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo8() {
            BookService service = new BookService();
            var list = service.GetList(10, 30, "a.id desc", 25);
            return Content(list.Count.ToString());
        }
        /// <summary>
        /// 演示调用功能的存储过程
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo9() { 
        //exec @ret=proc_demo1 @aa,@bb,@cc output 
            using (SqlConnection conn =
               new SqlConnection(DbSqlHelper.connstr))
            {
                conn.Open();//打开链接
                SqlCommand cmd = conn.CreateCommand();//创建命令
                //创建输入参数
                SqlParameter p_a = new SqlParameter("@a", 100);
                p_a.SqlDbType = SqlDbType.Int;//设置参数类型
                //默认就是输入参数
                p_a.Direction = ParameterDirection.Input;//表示输入参数
                
                SqlParameter p_b = new SqlParameter("@b", "abcd");
                p_b.SqlDbType = SqlDbType.VarChar;//设置参数类型
                p_b.Size = 50;

                SqlParameter p_c = new SqlParameter("@c", "");
                p_c.SqlDbType = SqlDbType.VarChar;//设置参数类型
                p_c.Size = 50;//设置字符varchar类型长度
                p_c.Direction = ParameterDirection.Output;//输出参数

                SqlParameter p_ret =new SqlParameter();
                p_ret.ParameterName = "@ret";//设置参数名
                p_ret.SqlDbType = SqlDbType.Int;//设置参数类型
                //设置参数类型为返回值参数
                p_ret.Direction = ParameterDirection.ReturnValue;
                //按照存储过程定义参数的顺序，添加参数
                
                cmd.Parameters.Add(p_ret);
                cmd.Parameters.Add(p_a);
                cmd.Parameters.Add(p_b);
                cmd.Parameters.Add(p_c);
                //设置cmd调用格式为存储过程
                cmd.CommandType = CommandType.StoredProcedure;
                //设置调用存储过程名字
                cmd.CommandText = "proc_demo1";
                //如果不返回结果集，就这样调用
                cmd.ExecuteNonQuery(); 
                string html = string.Format("<h1>输出参数值:{0},返回值{1}</h1>"
                    , p_c.Value, p_ret.Value
                    );
                //p_c.Value 检索某个输出参数值
             
                return Content(html);


            }

        }
        /// <summary>
        /// 这个案例演示 asp.net 验证框架使用
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo10() {
            //这里的视图，可以使用系统自动生成的。
            // 步骤： 选择强类型，类型里选择实体，支架模板选 create
            return View();
        }
        /// <summary>
        /// 自动映射到模型
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost] //处理提交          
        public ActionResult Demo10(Student student,HttpPostedFileBase file) {


            string filename;//文件名


            
            
            //服务端自定义校验
            //很多校验必须服务端完成，如验证用户名唯一性，验证码等
            //校验账号必须唯一
            if (student.Name == "admin1") { 
              //帐号存在
                //添加一个模型错误，参数1代表这个错误提示作用在哪个属性上
                ModelState.AddModelError("Name", "该名字已存在，不能注册");
            }
            //校验验证码是否正确,不区分大小写校验
            string code;
            if(TempData["code"]==null){
                code="errorerror";
            }
            else{
                code=TempData["code"].ToString().ToUpper();
            }
            if (student.Code.ToUpper() !=code ) {
                ModelState.AddModelError("code", "验证码输入错误");
            }
            //校验 班级必须选择
            if (student.ClassId == -1) {
                ModelState.AddModelError("classid", "必须选择一个班级");
            }




            if (file == null)
            {
                //没有上传文件
                ModelState.AddModelError("imgfile", "对不起，必须上传文件");

            }
            else
            {
                //上传文件了
                //判断文件的类型是否是图片
                //file.FileName 获得客户端上传的文件名
                if (BookShop.Tools.FileHelper.CheckFileName(file.FileName) == false)
                {
                    //不在允许范围内
                    ModelState.AddModelError("imgfile", "对不起，必须上传图片文件jpg,gif,png,bmp");

                }
                else
                {
                    //判断文件的长度是否符合要求
                    //file.ContentLength 文件长度，字节单位
                    // btye  1KB=1024B   1M=1024K
                    //如果长度超过100K，则不能上传
                    if (file.ContentLength / 1024 >= 100)
                    {
                        ModelState.AddModelError("imgfile", "对不起，长度必须小于100K");
                    }
                    else
                    {
                        //接下来，可以保存文件了
                        filename = BookShop.Tools.FileHelper.CreateFileName();//获得文件主名
                        //得到上传文件的扩展名，和主名一起拼接一个完整的文件名
                        filename = filename + "." + BookShop.Tools.FileHelper.GetExtFileName(file.FileName);
                        //把这个文件上传到 /uploads 目录下
                        //上传文件必须，路径必须是物理路径，类似 d:\a\b.jpg
                        //Server.MapPath()方法是将一个虚拟路径转换成物理路径
                        //这个方法以后会真很多地方使用。记住
                        string fullname = Server.MapPath("/upload/" + filename);
                        //开始真正保存文件了
                        try
                        {
                            file.SaveAs(fullname);//开始保存文件

                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("imgfile", "对不起，写文件失败");
                        }
                    }

                }
            }

            //校验是否通过
            if (ModelState.IsValid == true)
            {
                //开始处理注册逻辑
                return Content("<p>会员已经注册成功</p>");
            }
            else {
                return View();
                //return Content("<script>alert('提交数据非法，请检查');history.go(-1);</script>");
            }
         
        
        }
        /// <summary>
        /// 演示HTMLHELPer，视图助手
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo11() {
            return View();
        }
        /// <summary>
        /// 第五章:演示负责模型绑定之，模型中包含模型
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo12() {
            return View();
        
        }
        [HttpPost]
        public ActionResult Demo12( Book book)
        {
            return Content( book.Title+ book.Category.Name);

        }
        /// <summary>
        /// 第五章：绑定到集合对象作为模型
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo13() {
            return View();
        }
        [HttpPost]
        public ActionResult Demo13(List<Category> categories) {
            string html = "<ul>";
            foreach (Category c in categories) {
                html += "<li>" + c.Id + "    " + c.Name + "</li>";
            }
            html += "</ul>";
            return Content(html);
        }
        /// <summary>
        /// 第五章：绑定到模型中包含集合对象
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo14() {
            return View();
        }
        [HttpPost]
        public ActionResult Demo14(Order order) {
            return Content(order.PersonName + order.OrderBooks[0].Quantity);
        }
        /// <summary>
        /// 第5章，演示扩展方法
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo15() {
            //string name = "张三";
            //return Content(name.Hello("这是扩展方法"));
            return View();
        }
        /// <summary>
        /// 第5章，演示泛型类和泛型方法
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo16() {
            //实例化一个泛型类，注意必须在实例化时给泛型一个具体类型
            BookShop.Models.ClassDemo<Category> c1 = new Models.ClassDemo<Category>();
            string ret1 = c1.Show(new Category());
            //调用一个泛型方法，必须在调用时明确，泛型K是什么类型
            string ret2 = c1.Fun<Publisher>();
            return Content(ret1 + ret2);
        }
        /// <summary>
        /// 第五章：演示下拉列表，dripdownlist使用
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo17() {
            BookService service = new BookService();
            var list = service.GetList();//
            //如果下拉列表不需要请选择，这这句不要
            list.Insert(0, new Book { Id=-1,Title="--请选择图书--" });
            //创建下拉列表需要用到的数据源
            SelectList selectList = new SelectList(list, "Id", "Title");
            ViewBag.Books = selectList;
            return View();
        }
        /// <summary>
        /// 第六章：文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo18() {
            return View();
        }
        /// <summary>
        /// 文件上传是一个很大的极有可能引发漏洞的地方
        /// 为了降低被攻击风险，一般客户端上传文件后，都会把文件保存
        /// 在服务端的某个路径下，并且把文件名随机修改掉。一定要控制
        /// 好上传文件的类型以及长度
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file">form表单里有个type=file name=file的文件上传域，自动映射</param>
        /// <returns></returns>
        [HttpPost]       
        public ActionResult Demo18(string name,HttpPostedFileBase file) {
            if (file == null)
            {
                //没有上传文件
                Response.Write("<script>alert('对不起，必须上传文件');</script>");
                return View();
            }
            else
            {
                //上传文件了
                //判断文件的类型是否是图片
                //file.FileName 获得客户端上传的文件名
                if (BookShop.Tools.FileHelper.CheckFileName(file.FileName) == false)
                {
                    //不在允许范围内
                    Response.Write("<script>alert('对不起，必须上传图片文件jpg,gif,png,bmp');</script>");
                    return View();
                }
                else
                {
                    //判断文件的长度是否符合要求
                    //file.ContentLength 文件长度，字节单位
                    // btye  1KB=1024B   1M=1024K
                    //如果长度超过100K，则不能上传
                    if (file.ContentLength / 1024 >= 100)
                    {
                        Response.Write("<script>alert('对不起，长度必须小于100K');</script>");
                        return View();
                    }
                    else
                    {
                        //接下来，可以保存文件了
                        string filename = BookShop.Tools.FileHelper.CreateFileName();//获得文件主名
                        //得到上传文件的扩展名，和主名一起拼接一个完整的文件名
                        filename = filename + "." + BookShop.Tools.FileHelper.GetExtFileName(file.FileName);
                        //把这个文件上传到 /uploads 目录下
                        //上传文件必须，路径必须是物理路径，类似 d:\a\b.jpg
                        //Server.MapPath()方法是将一个虚拟路径转换成物理路径
                        //这个方法以后会真很多地方使用。记住
                        string fullname = Server.MapPath("/upload/" + filename);
                        //开始真正保存文件了
                        try
                        {
                            file.SaveAs(fullname);//开始保存文件
                            //这里文件保存成功了
                            // 通常情况，数据表中有个列，保存文件名，
                            //然后你可以把filename这个文件名保存到数据表里了
                            //这里假定会员保存成功了
                            string html = string.Format("<p>会员姓名:{0},"
                                + "<a href='{1}'><img src='{2}' /></a></p>", name
                                , "/upload/" + filename
                                , "/upload/" + filename);
                            return Content(html);
                        }
                        catch (Exception ex)
                        {
                            Response.Write("<script>alert('对不起，写文件失败');</script>");
                            return View();
                        }
                    }

                }




            }

        }
        /// <summary>
        /// 文件上传之批量上传
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo19()
        {
            return View();
        }

        //多个文件上传的关键点，在于，映射到后台，应该是一个数组
        //前台表单里，<input  type="file" name="files"/> 多个file
        //，name取值一样
        [HttpPost]
        public ActionResult Demo19(HttpPostedFileBase[] files)
        {
            string html = "<ul>";
            foreach (var file in files) { 
            //循环开始保存每个文件
                //具体做法，参考demo18
                //这里只是简单演示，显示每个上传的文件名
                html += "<li>" + file.FileName + "</li>";
            }
            html += "</ul>";
            return Content(html);
        }
        /// <summary>
        /// 比较复杂的表单元素提交
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo20() {
            return View();
        }
        [HttpPost]
        public ActionResult Demo20(string cname,string[] hobby)
        {
            return View();
        }
        /// <summary>
        /// 富文本编辑器，第六章
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo21() {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]//这个特性是指，取消安全检测功能
            //默认情况下,提交的数据里有<>是不可以的
        public ActionResult Demo21(string content) {
            return Content(content);
        }
        /// <summary>
        /// 委托进阶 第六章案例
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo22() {
            //什么叫委托？委托是一种特殊的类型，这个类型的实例是用来
            //指向一个方法，调用委托，间接调用方法
            MyMath math = new MyMath();
            //实例化一个委托对象，指向一个方法
            MyDelegate1 delegate1 = new MyDelegate1(math.Add);
            int ret = delegate1(20, 30);//调用委托，间接调用方法
            //return Content("ret=" + ret);
            //理解多播委托，如果委托的返回值是void，则该委托可以实现多播
            //即一个委托对象，可以指向多个方法，调用委托，这些方法都调用
            MyDelegate2 delegate2 = new MyDelegate2(math.Add1);
            delegate2 += new MyDelegate2(math.Minus1);//委托链表上再加个委托方法
            delegate2(100, 50);
            //return null;
            /////////////////////////////////////////////////
            //上面调用委托的方式，是属于C#2.0
            //c#3.0调用委托可以使用匿名方法
            MyDelegate1 delegate3 = delegate(int a, int b) {
                a = a + 100;
                return a + b;
            };
            ret = delegate3(50, 20);
          //  return Content("ret=" + ret);
            //C#4.0 调用委托可以使用拉姆达表达式
            //(a, b) 参数，类型是不需要写的，这里采用的是推断
           //如果只有一个参数，括号可以省略
            // 如果代码部分，只有一句代码，则{}可以省略,return 语句也可以省略
            // x=>x   (x)=>{return x;}
            MyDelegate1 delegate4 = (a, b) => a + b;
            ret=delegate4(80, 50);
            //return Content("ret=" + ret);

            /////////////////////////////////////////////////
            //这里演示一下 我们自己写的mywhere方法
            BookService service = new BookService();
            var list = service.GetList();//获得全部图书信息
            var list2 = list.MyWhere(p => p.UnitPrice >= 100);
            return Content("图书单价超过100元的图书共：" + list2.Count);














            //return View();
        }
        /// <summary>
        /// 演示 集合的常用扩展方法 ，在第9章
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo23() {
            BookService service = new BookService();
            var list = service.GetList();
            //这里返回值是IEnumerable 是一个接口，所有集合都实现该接口            
            // where title like '%计算机%' and unitprice>=50
            //title like '%计算机'  .EndsWith("计算机")
            //title like '计算机%'  .StartsWith("计算机")
            var list2 = list.Where(p => p.Title.Contains("计算机") && p.UnitPrice > 50);

            //select * from books where title like '%计算机%' and unitprice>=50
            // order by unitprice desc
            //OrderByDescending()降序，OrderBy() 升序
            var list3 = list
               .Where(p => p.Title.Contains("计算机") && p.UnitPrice > 50)
                         .OrderByDescending(p => p.UnitPrice);
            //select * from books where title like '%计算机%' and unitprice>=50
            // order by categoryid asc , unitprice desc
            //ThenByDescending() 可以多个排序条件，这是降序
            //ThenBy()是升序
            var list4 = list
             .Where(p => p.Title.Contains("计算机") && p.UnitPrice > 50)
                       .OrderBy(p => p.CategoryId)
                       .ThenByDescending(p => p.UnitPrice);
            //使用聚合函数
            // select count(*),avg(unptrice),max(unitprice),min(unitprice) from books
            //where title like '%计算机%'
            int count = list.Where(p => p.Title.Contains("计算机")).Count();
            decimal avg = list.Where(p => p.Title.Contains("计算机"))
                                 .Average(p => p.UnitPrice);
            var max = list.Where(p => p.Title.Contains("计算机"))
                                 .Max(p => p.UnitPrice);
            var min = list.Where(p => p.Title.Contains("计算机"))
                                 .Min(p => p.UnitPrice);
            string html = string.Format("<p>count={0},avg={1},max={2},min={3}</p>"
                   , count, avg, max, min);
          //  return Content(html);

            //分组统计
            //select categoryid,count(*),avg(unitprice)
            //from books where title like '%计算机%'  group by categoryid
            //这个返回值是 集合的集合 
            //IEnumerable<IGrouping<int,Book>>
            //int 是表示的groupby的键值
            //IGrouping是一个集合，这里表示的是每个图书类别的图书集合
            var list5 = list.Where(p => p.Title.Contains("计算机"))
                              .GroupBy(p => p.CategoryId);
            html = "<table>";
            html+="<tr>";
            html+="<td>图书类别编号</td><td>平均单价</td><td>图书数量</td>";
            html+="</tr>";
            foreach (var item in list5) {
                html += "<tr>";
                html += "<td>" + item.Key + "</td>";
                html += "<td>" + item.Count() + "</td>";
                html += "<td>" + item.Average(p=>p.UnitPrice)+ "</td>";
                html += "</tr>";
            }
            html += "</table>";
            //return Content(html);
            ///////////////////////////////////////////////
            // 分页处理
            //////////////////////////////////////
            int pageindex=3,pagesize=20 ;
            int skip=(pageindex-1)*pagesize;//跳过多少条
            int take=pagesize ;//取多少条
            //注意：再调用skip和take之前必须要先排序
            var list6 = list.Where(p => p.UnitPrice > 50)
                         .OrderBy(p => p.UnitPrice)
                         .Skip(skip).Take(take);
           // return View(list6);
            ////////////////////////////////////
            //取单条记录.Single() 如果差不多，抛异常
            //SingleOrDefault() 如果查不到，则返回null
           //First() 第一个 抛异常
            //list.FirstOrDefault() 第一个 没有则为null
            var book = list.SingleOrDefault(p => p.Id == 4939);
            return book == null ? Content("查不到") : Content(book.Title);


        }
        /// <summary>
        /// 演示EF框架
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo24() {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
                var list = db.Books.Where(p => p.UnitPrice > 50 && p.Title.Contains("ASP.NET"));
                //ef里的集合,dbSet<>,iQueryable<>  等，都是延时加载的
                //不用不加载，用了再加载，提高性能
               // List<> 立即加载，当调用ToList<>

            //查询语句,一般用查询方法比较多，查询语句还是别扭
                var list2 = from p in db.Books
                            where p.Title.Contains("ASP.NET")
                                && p.UnitPrice > 50
                                orderby p.UnitPrice descending
                            select p;
            ////////////////////////////////////////////////////////
            // 返回到ADO.NET，执行sql语句
                string sql = "select * from books";
            //这个返回的是dbsqlquery集合
                var list3 = db.Books.SqlQuery(sql);//执行sql查询
            //IEnumerable 这返回的是这个集合
                var list4 = db.Database
                    .SqlQuery<BookShop.Areas.Admin.Models.Books>(sql);

                /*select a.id,a.title as BookName ,b.name as Cname
    from Books  a 
    inner join Categories b
    on a.CategoryId=b.Id
                 * 
                 */
                sql = "select a.Id,a.title as BookName ,b.name as Cname "
                    + " from Books  a "
                    + " inner join Categories b "
                    + " on a.CategoryId=b.Id";
            //db.vw_books
            //当执行的sql语句中返回的列，没有任何一个实体类相对应的情况
            //下，则需要创建该类型
              
            //return View(list4);
            //==============================
            //执行DML语句，insert，delete，update
                sql = "delete from Publishers where ID=75";
                db.Database.ExecuteSqlCommand(sql); //返回受影响行数
            //给了Connection 对象，可以实现ADO.NET任何功能
            //实现事务，实现调用存储过程等
              //  var cmd = db.Database.Connection.CreateCommand();
            //cmd.ExecuteScalar() 返回首行首列
                //db.ExecuteScalar(sql); 这是我们扩展的方法
                return null;
            
        }
        /// <summary>
        /// 关于EF的延时加载和贪婪加载(立即加载）
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo25() {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            
                //配置延时加载属性为false

                db.Configuration.LazyLoadingEnabled = false;
                //使用include方法来预先加载关联对象
                var list = db.Books.Include("Categories").Where(p => p.UnitPrice > 40 && p.Title.Contains("ASP.NET"));
                //  var list2 = list.ToList();//立即加载
                return View(list);
            
        }
        /// <summary>
        /// jquery 学习
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo26() {
            return View();
        }
        /// <summary>
        /// jquery 学习2  演示json
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo27() {
            return View();
        }
        /// <summary>
        /// ajax原理
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo28() {
            return View();
        }
        /// <summary>
        /// ajax请求，检测用户名是否可以注册
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Demo28_1(string name)
        {
            string json;
            if (name == "admin")
            {
                json = "{\"success\":1}";
            }
            else {
                json = "{\"success\":0}";
            }
            return Content(json);
        }
        /// <summary>
        /// ajax请求，校验验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Demo28_2(string code)
        {
            string json;
            if (code == TempData["code"].ToString())
            {
                json = "{\"success\":0}";
            }
            else
            {
                json = "{\"success\":1}";
            }
            return Content(json);
        }
        /// <summary>
        /// ajax调用，返回图书类别json格式数组文本
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo28_3() {
            CategoryService serivce = new CategoryService();
            var list = serivce.GetList();//得到对象
            //将对象转换成json格式，返回给客户端
            return Json(list, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        /// jquery ajax方法之 get
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo29() {
            return View();
        }
        public ActionResult Demo29_1()
        {
            CategoryService serivce = new CategoryService();
            var list = serivce.GetList();//得到对象
            //将对象转换成json格式，返回给客户端
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// ajax请求，返回图书类别下图书信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public ActionResult Demo29_2(int cid) {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            //使用ef框架的话，取出的集合要用select方法来过滤需要返回的列
            //最后要转换成list集合
            var list = db.Books.Where(p => p.CategoryId == cid)
                         .Select(p => new
                         {
                             p.Id,
                             p.Title,
                             Cname = p.Categories.Name,
                             p.UnitPrice
                         })
                        .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ajax请求，删除图书
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult Demo29_3(int bookid) {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
       
            BookShop.Areas.Admin.Models.Books book = new Areas.Admin.Models.Books();// c = new Categories { Id = id };
            book.Id = bookid;
            db.Books.Attach(book);//给定实体附加到上下文中
            db.Books.Remove(book);//删除
            string json;
            try
            {
                db.SaveChanges();//提交到数据库，完成修改
                json = "{\"success\":0}";
            }
            catch (Exception ex) {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        /// <summary>
        /// jquery ajax方法演示 
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo30() {
            return View();
        }
        /// <summary>
        /// ajax调用，返回用户是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Demo30_1 (string name){
            using (BookShop.Areas.Admin.Models.BookShopPlusEntities db
                = new Areas.Admin.Models.BookShopPlusEntities()) {
                    string html;
                //如果返回的html是比较简单的，直接拼接
                    var user = db.Users.SingleOrDefault(u => u.LoginId == name);
                    if (user != null)
                    {
                        //存在
                        html = "<span style='color:red'>该用户已存在，不能注册</span>";
                    }
                    else {
                        html = "<span style='color:green'>该用户已不存在，可以注册</span>";
                    }
                    return Content(html);
            
            }
        }

        public ActionResult Demo30_2(string code)
        {
            string json;
            if (code != TempData["code"].ToString())
            {
                json = "<span style='color:red'>验证码错误</span>";
            }
            else
            {
                json = "<span style='color:green'>验证码正确</span>";
            }
            return Content(json);
        }
        /// <summary>
        /// ajax调用，返回图书信息
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public ActionResult Demo30_3(string title) {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            var books = db.Books.Where(p => p.Title.Contains(title));
            //需要生成一大堆负责的html脚本
            //那就是用分部视图技术来完成
            return PartialView(books);
        }

        public ActionResult Demo30_4(int bookid)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();

            BookShop.Areas.Admin.Models.Books book = new Areas.Admin.Models.Books();// c = new Categories { Id = id };
            book.Id = bookid;
            db.Books.Attach(book);//给定实体附加到上下文中
            db.Books.Remove(book);//删除
            string json;
            try
            {
                db.SaveChanges();//提交到数据库，完成修改
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        /// <summary>
        /// ajax演示，最复杂的，功能最全面的ajax方法演示
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo31() {
            return View();
        
        }
        public ActionResult Demo31_1() {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            var list = db.Categories
                .Select(p => new {p.Id,p.Name })
                .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);

        

        }
        /// <summary>
        /// ajax调用，查询图书
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public ActionResult Demo31_2(string title,int cid=0)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            System.Text.StringBuilder sqlwhere = 
                new System.Text.StringBuilder(" where 1=1 ");
            if (cid > 0) {
                sqlwhere.Append(" and categoryid=" + cid);

            }
            if (string.IsNullOrEmpty(title) == false) {
                sqlwhere.Append(string.Format(" and title like '%{0}%'", title));
            }
            string sql = "select * from books " + sqlwhere.ToString();
            var books = db.Books.SqlQuery(sql);
            //var books = db.Books.Where(p => p.Title.Contains(title));
            //需要生成一大堆负责的html脚本
            //那就是用分部视图技术来完成
            return PartialView(books);
        }
        /// <summary>
        /// 演示.NET MVC中ajax处理
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo32() {
            //下面是构造图书类别下拉列表数据源
            CategoryService service = new CategoryService();
            var list = service.GetList();
            list.Insert(0, new Category {Id=-1,Name="--请选择图书类别--" });
            SelectList categories = new SelectList(list, "Id", "Name");
            ViewBag.Categoies = categories;
            return View();
        }
        /// <summary>
        /// ajax请求，查询图书
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public ActionResult Demo32_1(string title, int cid = 0)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            System.Text.StringBuilder sqlwhere =
                new System.Text.StringBuilder(" where 1=1 ");
            if (cid > 0)
            {
                sqlwhere.Append(" and categoryid=" + cid);

            }
            if (string.IsNullOrEmpty(title) == false)
            {
                sqlwhere.Append(string.Format(" and title like '%{0}%'", title));
            }
            string sql = "select * from books " + sqlwhere.ToString();
            var books = db.Books.SqlQuery(sql);
            //var books = db.Books.Where(p => p.Title.Contains(title));
            //需要生成一大堆负责的html脚本
            //那就是用分部视图技术来完成
            return PartialView(books);
        }

        public ActionResult Demo32_2(int id)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();

            BookShop.Areas.Admin.Models.Books book = new Areas.Admin.Models.Books();// c = new Categories { Id = id };
            book.Id = id;
            db.Books.Attach(book);//给定实体附加到上下文中
            db.Books.Remove(book);//删除
            string json;
            try
            {
                db.SaveChanges();//提交到数据库，完成修改
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":-1}";
            }
            return Content(json);
        }
        /// <summary>
        /// ajax完整案例，实现出版社管理功能
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo33() {
            return View();
        
        }
        public ActionResult Demo33_1(string name,int pageindex=1)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            System.Text.StringBuilder sqlwhere =
                new System.Text.StringBuilder(" where 1=1 ");
          
            if (string.IsNullOrEmpty(name) == false)
            {
                sqlwhere.Append(string.Format(" and name like '%{0}%'", name));
            }
            string sql = "select * from publishers " + sqlwhere.ToString();
            int pagesize = 5;//页尺寸
            int skip = (pageindex - 1) * pagesize;

            var pubs = db.Publishers.SqlQuery(sql)
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(pagesize);
            var recordcount = db.Publishers.SqlQuery(sql).Count();//总记录
            var pagecount = recordcount % pagesize == 0 ? recordcount / pagesize : recordcount / pagesize + 1;
            ViewBag.RecordCount = recordcount;
            ViewBag.PageCount = pagecount;
            
           
            //var books = db.Books.Where(p => p.Title.Contains(title));
            //需要生成一大堆负责的html脚本
            //那就是用分部视图技术来完成
            return PartialView(pubs);
        }
        /// <summary>
        /// ajax调用，完成新增出版社
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Demo33_2(string name) {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            BookShop.Areas.Admin.Models.Publishers p = new Areas.Admin.Models.Publishers();
            p.Name = name;
            db.Publishers.Add(p);
            string json;
            try
            {
                db.SaveChanges();
                json = "{\"success\":0}";
            }
            catch (Exception ex) {
                json = "{\"success\":-1}";
            }
            return Content(json);
       
        }
        /// <summary>
        /// ajax调用，得到出版社对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Demo33_3(int id)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            var pub = db.Publishers.Where(p => p.Id == id)
                .Select(p => new { p.Id, p.Name }).First();
            return Json(pub, JsonRequestBehavior.AllowGet);
                

        }

        public ActionResult Demo33_4(BookShop.Areas.Admin.Models.Publishers p)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
          
            db.Entry(p).State = EntityState.Modified;
            string json;
            try
            {
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
        /// easyui 插件 datagrid
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo34() {
            return View();
        }
        /// <summary>
        /// ajax调用，加载图书类别
        /// int page,int rows 
        /// easyui datagrid插件，因为启用了分页，传入的数据
        /// page 代表页索引 rows代表，每页多少条记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo34_1(int page,int rows)
        {
         /*   {
"total":28,
"rows":[{},{},{}]
        }*/

            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            int skip = (page - 1) * rows;
            var list = db.Categories.OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Name })
                        .Skip(skip)
                        .Take(rows)
                        .ToList();
            int count = db.Categories.Count();//得到总记录
            //使用第三方类库，将对象转换成json格式字符串
            string json = LitJson.JsonMapper.ToJson(list);
          
            string retJson=string.Format("{0}\"total\":{1},\"rows\":{2}{3}"
                     ,"{",count,json,"}"
                );
            return Content( retJson);
        }
        /// <summary>
        /// easyui 插件datagrid复杂案例
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo35()
        {
            return View();
        }
        //string sort  按照哪个列排序,string order  排序规则 asc或则 desc
        public ActionResult Demo35_1(string sort,string order ,
        int page,int rows,string title, int cid = 0)
        {
            BookShop.Areas.Admin.Models.BookShopPlusEntities db = new Areas.Admin.Models.BookShopPlusEntities();
            int skip = (page - 1) * rows;
            System.Text.StringBuilder sqlwhere =
                new System.Text.StringBuilder(" where 1=1 ");
            if (cid > 0)
            {
                sqlwhere.Append(" and categoryid=" + cid);
            }
            if (string.IsNullOrEmpty(title) == false)
            {
                sqlwhere.Append(string.Format(" and title like '%{0}%'", title));
            }
            string sql = "select * from books " + sqlwhere.ToString()
                  +" order by "+sort+" "+order;
            var books = db.Books.SqlQuery(sql)                       
                       .Select(p =>
                           new { p.Id,p.Title,
                           p.UnitPrice,Cname=p.Categories.Name})
                           .Skip(skip)
                           .Take(rows)
                          .ToList() ;
            int count = db.Books.SqlQuery(sql).Count();//得到总记录
            //使用第三方类库，将对象转换成json格式字符串
            string json = LitJson.JsonMapper.ToJson(books);            
            string retJson = string.Format("{0}\"total\":{1},\"rows\":{2}{3}"
                     , "{", count, json, "}"
                );
            return Content(retJson);            
        }
        /// <summary>
        /// easyui插件，表单插件和验证插件使用
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo36() {
            return View();
        }
        public ActionResult Demo36_1(string name, string password, int score) { 
            //这里应该是执行插入逻辑，根据结果返回json格式
            string json = "{\"success\":0,\"name\":\"" + name + "\"}";
            return Content(json);
        }
        /// <summary>
        /// 演示缓存机制，页面缓存
        /// 这个特性表示启用缓存，缓存时间为20秒，
        /// VaryByParam="none" 表示缓存无参数，某些情况如
        ///  /demo/demo37?id=1  如果想根据每个id参数产生一个缓存版本
        ///  这需要VaryByParam="id" 可以多个参数，逗号间隔
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration=20,VaryByParam="none")]
        public ActionResult Demo37() {
            return View();
        }
        /// <summary>
        /// 演示自定义过滤器
        /// </summary>
        /// <returns></returns>
        [MyFielterAction] //这是自定义的过滤器，作用在动作上
        [MyFilterView] //这个是自定义过滤器，作用在视图上
        public ActionResult Demo38() {
            Response.Write("<p>动作开始执行了</p>");
            return View();
        }
        /// <summary>
        /// 演示EXCEL的交互.导入和导出
        /// 这是常见功能，我们需要把数据导出到EXCEL中，做报表。
        /// 演示导出
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo39() {
            CategoryService service = new CategoryService();
            var list = service.GetList();
            return View(list);
        }
        /// <summary>
        /// ajax调用，导出数据到EXCEL
        /// 1.网站先添加对EXCEL组件库的引用  
        /// 2.导入命名空间
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo39_1() {
            CategoryService service = new CategoryService();
            var list = service.GetList();
            //Application excel应用程序
            //Workbook 工作簿
            //Worksheet 工作表

            Application app = new Application();//创建一个EXCEL应用程序对象
            //app.Visible = true;//让这个应用程序可见，调试是用，正常应该不可见
            //创建一个新的工作簿文件，注意，参数是模板，这里不用模板
            Workbook book = app.Workbooks.Add();
            //得到这个工作簿中名字为sheet1的工作表
            Worksheet sheet1 = book.Sheets.get_Item("sheet1");
            //excel中 B7表示，第七行的第B列 
            //注意，在EXCEL集合是从下标1开始
            //下面写下表头
            //定义一个范围，选中第2行第2列，就是B2
            Range cell = (Range)sheet1.Cells[2, 2];
            cell.Value = "类别编号";//写文字到单元格里
            cell = (Range)sheet1.Cells[2, 3];
            cell.Value = "类别名称";
            /////////////////////////////////////////////
            //下面循环这个集合，把数据依次往下写
            int row = 3;
            foreach (Category c in list) {
                cell = (Range)sheet1.Cells[row, 2];
                cell.Value = c.Id;
                cell = (Range)sheet1.Cells[row, 3];
                cell.Value = c.Name;
                row++;
            }
            //创建一个excel的文件名
            string file = Tools.FileHelper.CreateFileName()+".xlsx";
            //保存文件需要用到物理路径
            string filename = Server.MapPath("~/excelfiles/" + file);
            string html = "";
            try
            {
                book.SaveAs(filename);//保存文件
                html = string.Format("<a href='{0}'>下载EXCEL</a>",
                                                   "/excelfiles/" + file);
               
            }
            catch (Exception ex)
            {
                html = "<span stryle='font-weight:bold'>" + ex.Message + "</span>";
            }
            finally {
                book.Close();//关闭工作簿
                app.Quit();//退出excel应用程序
            
            }
            return Content(html);

        }
        /// <summary>
        /// 思路，先把上传的EXCEL文件，保存，然后打开操作
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ActionResult Demo39_2(HttpPostedFileBase file) {
            string json="";
            //这里不验证文件了，应该要校验文件的类型
            if (file != null)
            {
                string filename = Tools.FileHelper.CreateFileName() + ".xlsx";
                string fullname = Server.MapPath("~/upload/" + filename);//物理路径
                file.SaveAs(fullname);//保存文件
                //然后EXCEL操作，打开保存的文件
                Application app = new Application();//创建一个EXCEL应用程序对象
             //   app.Visible = true;//让这个应用程序可见，调试是用，正常应该不可见
                //注意，参数是模板，这里不用模板
                //打开一个EXCEL工作簿
                Workbook book = app.Workbooks.Open(fullname);
                //得到这个工作簿中名字为sheet1的工作表
                Worksheet sheet1 = book.Sheets.get_Item("sheet1");
                int row = 3;
                List<Category> list = new List<Category>();
                Range cell = (Range)sheet1.Cells[row, 2];//得到编号数据列
                //如果有值
                while (cell.Value!=null)
                {
                    Category c = new Category();
                    c.Id = Convert.ToInt32(cell.Value);
                    cell = (Range)sheet1.Cells[row, 3];
                    c.Name = cell.Value.ToString();
                    list.Add(c);
                    row++;
                    cell = (Range)sheet1.Cells[row, 2];//得到编号数据列
                }
                book.Close();//关闭工作簿
                app.Quit();//退出excel应用程序
                //下面，应该是将集合里的数据添加到数据库中。这里省略插入过程
                json = "{\"success\":0,\"count\":" + list.Count + "}";
            }
            else {
                json = "{\"success\":-1}";
            }
            return Content(json);
        
        
        }

        /// <summary>
        /// 演示C#中反射机制
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo40() {
            //反射技术：动态加载程序集，动态创建成员对象，动态调用成员
            //Assembly 程序集类
            //Type 这个类用来描述类型 类
            //PropertyInfo 这个类用来描述属性
            //MethodInfo 这个类用来描述方法
            string file=Server.MapPath("~/bin/entity.dll");//得到物理路径
            //Assembly assembly = Assembly.LoadFile(file);//加载这个程序集
            //assembly.FullName 程序集的全名
            string fullname = "Entity, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            Assembly assembly = Assembly.Load(fullname);//加载这个程序集
            //Entity, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            //也可以通过加载全名的方式来加载程序集，前提是该程序集已经存在网站的bin目录中
            var types = assembly.GetTypes();//获得该程序集中所有的类型
            string html = "";
            foreach (Type t in types) {
                html += string.Format("<li>{0}.{1}</li>", t.Namespace,t.Name);
            }
            //Response.Write(html);
            //可以根据类的全名来得到指定类型
            Type type = assembly.GetType("Accp.BookShop.Entity.Publisher");
            //得到所有实例的公有属性
            var props = type.GetProperties();
            html = "";
            foreach (var p in props) { 
                html+=string.Format("<li>{0} {1}</li>",
                    p.Name,p.PropertyType.ToString());
            }
            var methods = type.GetMethods();//得到所有方法
            html = "";
            foreach (MethodInfo m in methods) {
                html += string.Format("<li>{0} {1}</li>",
                m.ReturnType.ToString(), m.Name);
            
            }
           // return Content(html);
            //////////////////////////////////////////////////////////////////////////
            // 演示动态，创建对象，调用成员
            //通过类的全名创建
            //Publisher p=new Publisher();
            var obj = assembly.CreateInstance("Accp.BookShop.Entity.Publisher");            
            Type t2 = obj.GetType();//得到obj这个对象的type对象
            //
            //获得这个类型里的指定name属性
            PropertyInfo pname = t2.GetProperty("Name");
            //给obj对象的name属性赋值 相当于p.Name="北大";
            pname.SetValue(obj, "北大青鸟");//给属性赋值
            //获取属性值 
            //得到obj对象上的name属性值
            string str =Convert.ToString( pname.GetValue(obj));
            //下面演示调用成员方法
            MethodInfo tostring = t2.GetMethod("ToString");//得到指定的方法
            string ret = Convert.ToString(tostring.Invoke(obj, null));//调用指定的方法
            return Content(ret);
            //return View();
        }
        /// <summary>
        /// 测试下反射机制的实际案例
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo41() {
            string sql = "select * from publishers";
            var ds = Accp.Tools.DbSqlHelper.Query(sql);//获得dataset
            var list = Accp.Tools.ClassHelper
                .ConvertDataTableToList<Publisher>(ds.Tables[0]);
            string json = "{\"Id\":1,\"Name\":\"北大青鸟\"}";
            Publisher p = Accp.Tools.ClassHelper.ConvertJsonToObject<Publisher>(json);
           ////////////////////
            Category c = new Category {  Id=10, Name="计算机", SortNum=1};
            
            return Content(Accp.Tools.ClassHelper.GetInsertSql(c,"Id"));
            //return View(list);
        }
        /// <summary>
        /// 特性：是一种特殊的类，这种类是用来修饰类型，成员的
        /// 一般是通过反射技术来获得加在成员上的特性，
        /// 以此做一系列的业务处理 
        /// 系统定义了很多特性类，如加在action上，[httppost],过滤器等
        /// 上次课，反射技术中，getInsertSql，有个弊端，表名和类名，字段名和属性名是绑定的
        /// 需要改进，能够灵活处理，即表名和类名不绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult Demo42() {
            Bus bus = new Bus { Id = 123, Name = "金龙", Supply = "上汽公司" };
            Accp.Orm.DbContent db = new Accp.Orm.DbContent();
            return Content(db.GetInsertSql(bus));
        
        }
    }
}








