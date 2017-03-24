using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;
using System.Text;
using System.Data;
using BookShop.Models.Services;

namespace BookShop.Controllers
{
    /// <summary>
    /// 作者:李天页
    /// 功能描述:兑换积分控制器
    /// </summary>
    public class ExchangeIntegralController : Controller
    {
        //
        // GET: /ExchangeIntegral/

        private BookShopPlusEntities db = new BookShopPlusEntities();


        /// <summary>
        /// 作者:李天页
        /// 功能描述:首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            new UserService().CheckUser();
            //获得可用积分
            //var id=Convert.ToInt32(Session["id"]);
            int userid = new UserService().GetUserId(Session["name"].ToString());
            var integral = db.Users.Find(userid).scoreCurrent;
            ViewBag.Score = integral;

            //获得兑换比例，1（钱）：1（积分）,只取用：后的数字
            var proportion = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralNoBuy")).ToList()[0].ValueField;
            var propInte = Convert.ToInt32(Convert.ToString(proportion).Split(':')[1]);
            ViewBag.PropInte = propInte;

            return View();
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:动态根据用户输入的要兑换的积分，算出需要多少钱，供ajax调用
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public ActionResult GetNeedMoney(int num)
        {
            //获得兑换比例，1（钱）：1（积分）,只取用：后的数字
            var proportion = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralNoBuy")).ToList()[0].ValueField;
            var propInte = Convert.ToDecimal(Convert.ToString(proportion).Split(':')[1]);

            var money = Convert.ToDecimal(num) / propInte;

            var value = string.Format("{0:c}", money);

            return Content(value);
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:添加积分，供ajax调用
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public ActionResult AddInte(int num)
        {
            using (var con = db.Database.Connection)
            {
                con.Open();
                var cmd = db.Database.Connection.CreateCommand();
                var tran = con.BeginTransaction();
                cmd.Transaction = tran;//挂接事务              
                StringBuilder stb = new StringBuilder();
                string json = "";
           
                try
                {                  
                    //增加用户可用积分                  
                    stb.AppendLine(" update Users set scoreCurrent=scoreCurrent+");
                    stb.AppendFormat(" {0}", num);
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行扣除积分


                    //增加用户总积分
                    stb.Clear();
                    stb.AppendLine(" update Users set scoreTotal=scoreTotal+");
                    stb.AppendFormat(" {0}", num);
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行扣除积分
                    //获得用户id
                    //int userid = Convert.ToInt32(Session["id"]);
                    int userid = new UserService().GetUserId(Session["name"].ToString()); 
                    //获得积分历史描述
                    string Descriptions = "'这是您于" + System.DateTime.Now.Year + "年" + System.DateTime.Now.Month + "月"
                        + System.DateTime.Now.Day + "日执行的钱币直接兑换积分,积分增加" + num + "分'";
                    //插入积分历史
                    stb.Clear();
                    stb.AppendLine(" insert into ScoreHistory(CreateTime,UserId,Flag,SingleScore,Descriptions) values");
                    stb.AppendFormat(" ('{0}',{1},{2},{3},{4})", System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
                        userid, 3, num, Descriptions);
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行插入明细


                    //更新会员等级，调用存储过程
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "proc_exec_UserScore";
                    cmd.ExecuteNonQuery();

                    //代码到这里，没有错误的话，应该执行提交事务
                    tran.Commit();

                    json = "{\"success\":0}";
                    return Content(json);
                }
                catch (Exception ex)
                {
                    //有异常
                    tran.Rollback();//回滚事务
                    json = "{\"success\":1}";
                    return Content(json);
                }
            }
        }



    }
}
