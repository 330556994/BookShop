using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShop.Areas.Admin.Models;
using System.Data.SqlClient;
using Accp.Tools;
using System.Text;
using System.Data;




namespace BookShop.Models.Services
{
    /// <summary>
    /// 作者:KL
    /// 功能描述:订单业务类
    /// 创建日期:2016-10-21
    /// </summary>
    public class OrderService
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();     

        /// <summary>
        /// 作者:李天页
        /// 功能描述:添加一个订单，这里需要启用事务
        /// 创建日期:2016-10-21
        /// </summary>
        /// <param name="order"></param>
        /// <returns>订单编号</returns>
        public int Add(Orders order)
        {           
            using (var con=db.Database.Connection) 
            {
                con.Open();
                var cmd = db.Database.Connection.CreateCommand();
                var tran =con.BeginTransaction() ;             
                cmd.Transaction = tran;//挂接事务


                //插入订单表数据
                StringBuilder stb = new StringBuilder(" insert into Orders");
                stb.AppendLine(" (OrderDate,UserId,TotalPrice,flag,personName,phone,address,sendCash,OrderRemark)");
                stb.AppendLine(" values");
                stb.AppendFormat(" ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    order.OrderDate,order.UserId,order.TotalPrice,
                    order.flag,order.personName,order.phone,
                    order.address,order.sendCash,order.OrderRemark);

                int orderId;//接收刚才插入的主键列数据
                try
                {
                    cmd.CommandText=stb.ToString();
                    cmd.ExecuteNonQuery();//执行插入订单
                    stb.Clear();
                    stb.AppendLine(" select @@IDENTITY");
                    cmd.CommandText=stb.ToString();
                    orderId=Convert.ToInt32(cmd.ExecuteScalar());



                    //插入订单明细开始，循环遍历订单明细集合
                    foreach(OrderBook book in order.OrderBook )
                    {
                        stb.Clear();
                        stb.AppendLine(" insert into OrderBook(OrderID,BookID,Quantity,UnitPrice) values");
                        stb.AppendFormat(" ({0},{1},{2},{3})",orderId,book.BookID,book.Quantity,book.UnitPrice);
                        cmd.CommandText=stb.ToString();
                        cmd.ExecuteNonQuery();//执行插入明细
                    }



                    //循环遍历要扣除的总积分
                    decimal totalPayMent=0;
                    //插入积分订单明细开始
                    foreach (OrderIntegral inte in order.OrderIntegral)
                    {
                        stb.Clear();
                        stb.AppendLine(" insert into OrderIntegral(OrderID,IntegralGoodsID,Quantity,PaymentIntegral) values");
                        stb.AppendFormat(" ({0},{1},{2},{3})", orderId, inte.IntegralGoodsID, inte.Quantity, inte.PaymentIntegral);
                        cmd.CommandText = stb.ToString();
                        cmd.ExecuteNonQuery();//执行插入明细
                        totalPayMent += inte.PaymentIntegral;
                    }              



                    //扣除用户积分
                    stb.Clear();
                    stb.AppendLine(" update Users set scoreCurrent=scoreCurrent-");
                    stb.AppendFormat(" {0}", totalPayMent);
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行扣除积分

                   
                    //获得用户id
                    int userid = new UserService().GetUserId(HttpContext.Current.Session["name"].ToString());     
                    //获得积分历史描述
                    string Descriptions="'这是您于"+System.DateTime.Now.Year+"年"+System.DateTime.Now.Month+"月"
                        +System.DateTime.Now.Day+"日执行的随订单兑换积分商品,积分减去"+totalPayMent+"分'";
                    //插入积分历史
                    stb.Clear();
                    stb.AppendLine(" insert into ScoreHistory(CreateTime,UserId,Flag,SingleScore,Descriptions) values");
                    stb.AppendFormat(" ('{0}',{1},{2},{3},{4})", System.DateTime.Now,
                        userid,5,totalPayMent-totalPayMent*2,Descriptions);
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行插入明细
                    

                    //代码到这里，没有错误的话，应该执行提交事务
                    tran.Commit();
                    return orderId;
                }
                catch(Exception ex)
                {
                    //有异常
                    tran.Rollback();//回滚事务
                    throw ex;//跑出异常，由外界捕获再处理
                }
            }
        }



        /// <summary>
        /// 特殊订单方法
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int SpecialAdd(Orders order)
        {
            using (var con = db.Database.Connection)
            {
                con.Open();
                var cmd = db.Database.Connection.CreateCommand();
                var tran = con.BeginTransaction();
                cmd.Transaction = tran;//挂接事务


                //插入订单表数据
                StringBuilder stb = new StringBuilder(" insert into Orders");
                stb.AppendLine(" (OrderDate,UserId,TotalPrice,flag,personName,phone,address,sendCash,OrderRemark)");
                stb.AppendLine(" values");
                stb.AppendFormat(" ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    order.OrderDate.ToString("yyyy/MM/dd hh:mm:ss"), order.UserId, order.TotalPrice,
                    order.flag, order.personName, order.phone,
                    order.address, order.sendCash, order.OrderRemark);

                int orderId;//接收刚才插入的主键列数据
                try
                {
                    cmd.CommandText = stb.ToString();
                    cmd.ExecuteNonQuery();//执行插入订单
                    stb.Clear();
                    stb.AppendLine(" select @@IDENTITY");
                    cmd.CommandText = stb.ToString();
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());




                    //插入订单明细开始，循环遍历订单明细集合
                    foreach (OrderBook book in order.OrderBook)
                    {
                        stb.Clear();
                        stb.AppendLine(" insert into OrderBook(OrderID,BookID,Quantity,UnitPrice,flag) values");
                        stb.AppendFormat(" ({0},{1},{2},{3},{4})", orderId, book.BookID, book.Quantity, book.UnitPrice,book.flag);
                        cmd.CommandText = stb.ToString();
                        cmd.ExecuteNonQuery();//执行插入明细
                    }
                                                 

                    //代码到这里，没有错误的话，应该执行提交事务
                    tran.Commit();
                    return orderId;
                }
                catch (Exception ex)
                {
                    //有异常
                    tran.Rollback();//回滚事务
                    throw ex;//跑出异常，由外界捕获再处理
                }
            }
        }


      
        

    /*

        /// <summary>
        /// 作者:KL
        /// 功能描述:根据当前登录用户id查看全部的订单
        /// 供OrderController中index（）调用
        /// 创建日期:2016-10-24
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public List<Order> GetOrderByUserId(int userId)
        {
            List<Order> orderList = new List<Order>();
            StringBuilder stb = new StringBuilder(" SELECT * FROM Orders");
            stb.AppendFormat(" WHERE UserId={0} ORDER BY OrderDate",userId);
            var ds = DBHelp.Query(stb.ToString());
            foreach(DataRow row in ds.Tables[0].Rows )
            {
                Order order = new Order();
                order.Id = Convert.ToInt32(row["Id"]);
                order.OrderDate = Convert.ToDateTime(row["OrderDate"]);
                order.UserId = Convert.ToInt32(row["UserId"]);
                order.TotalPrice = Convert.ToDecimal(row["TotalPrice"]);
                order.Flag = Convert.ToInt32(row["flag"]);
                order.PersonName = Convert.ToString(row["personName"]);
                order.Phone = Convert.ToString(row["phone"]);
                order.Address = Convert.ToString(row["address"]);
                order.SendCash = Convert.ToDecimal(row["sendCash"]);
                order.UserCancelReason = Convert.ToString(row["userCancelReason"]);
                order.OrderRemark = Convert.ToString(row["OrderRemark"]);
                orderList.Add(order);
            }
            return orderList;
        }



        /// <summary>
        /// 作者:KL
        /// 功能描述:根据订单id查看详情
        /// 创建日期:2016-10-24
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<OrderBook> GetOrderBookByOrderId(int id)
        {
            List<OrderBook> list = new List<OrderBook>();        
            string sql = string.Format(" SELECT * FROM OrderBook WHERE OrderID={0}", id);        
            var ds = DBHelp.Query(sql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                OrderBook book = new OrderBook();
                book.Id = Convert.ToInt32(row["Id"]);
                book.OrderID = Convert.ToInt32(row["OrderID"]);
                book.BookID = Convert.ToInt32(row["BookID"]);
                book.Quantity = Convert.ToInt32(row["Quantity"]);
                book.UnitPrice = Convert.ToDecimal(row["UnitPrice"]);
                list.Add(book);
            }
            return list;
        }




        /// <summary>
        /// 作者:KL
        /// 功能描述:使用存储过程proc_completeOrder
        /// 如果该订单的状态为3 则，将状态修改成6,
        /// 一元等于1积分
        /// 将积分更新到会员的总积分上，注意，加了总积分
        /// 很有可能引起等级提升，在提升该会员等级。
        /// 存储过程返回值：0 成功 -1代表订单状态无法处理
        /// 创建日期:2016-10-29
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <returns>0 成功 -1代表订单状态无法处理</returns>
        public int CompleteOrder(int orderId)
        {          
            //这里演示如何调用返回结果集的存储过程
            using(SqlConnection con=new SqlConnection (DBHelp.connstr))
            {
                //s打开连接
                con.Open();
                //创建命令
                SqlCommand cmd = con.CreateCommand();

                cmd.Parameters.AddWithValue("@orderid", orderId);
                cmd.Parameters.Add("@ret",SqlDbType.Int,50).Direction=ParameterDirection.ReturnValue;
   
                ///////////////////////////////////////////////////////

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "proc_completeOrder";

                cmd.ExecuteNonQuery();

                return Convert.ToInt32(cmd.Parameters["@ret"].Value);
            }
        }




        /// <summary>
        ///  作者:KL
        /// 功能描述:不使用存储过程
        /// 如果该订单的状态为3 则，将状态修改成6,
        /// 一元等于1积分
        /// 将积分更新到会员的总积分上，注意，加了总积分
        /// 很有可能引起等级提升，在提升该会员等级。
        /// 存储过程返回值：0 成功 -1代表订单状态无法处理
        /// 创建日期:2016-10-29
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="userId">所属会员编号</param>
        /// <param name="flag">订单状态</param>
        /// <param name="totalPrice">订单商品价格，不含运费</param>
        /// <param name="scoretotal">总积分</param>
        /// <param name="userroleId">等级编号</param>
        /// <returns></returns>
        public int CompleteOrder2(int orderId)
        {
            using (SqlConnection con = new SqlConnection(DBHelp.connstr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                SqlCommand cmd = con.CreateCommand();//创建command
                cmd.Transaction = tran;//挂接事务
                StringBuilder stb = new StringBuilder(" select userid,flag,totalprice");
                stb.AppendFormat(" from orders where ID={0}",orderId);
                                  
                SqlDataAdapter da;
                DataSet ds;
                int ret = -1;


                try
                {
                    //第一句sql语句，并判断该订单状态是否为3
                    cmd.CommandText = stb.ToString();
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["flag"]) != 3)
                    {
                        return ret;
                    }

                    //对变量赋值
                    int userid = Convert.ToInt32(ds.Tables[0].Rows[0]["userid"]);
                    decimal totalprice = Convert.ToDecimal(ds.Tables[0].Rows[0]["totalprice"]);


                    //第二句sql语句
                    stb.Clear();
                    stb.AppendFormat(" update Orders set flag=6 where ID={0}", orderId);
                    cmd.CommandText = stb.ToString();
                    ret = Convert.ToInt32(cmd.ExecuteNonQuery()) != 1 ? -1 : 0;

                    //第三句sql语句
                    stb.Clear();
                    stb.AppendFormat(" update Users set scoreCurrent=scoreCurrent+{0}",totalprice);
                    stb.AppendFormat(" ,scoreTotal=scoreTotal+{0} where ID={1}", totalprice, userid);
                    cmd.CommandText = stb.ToString();
                    ret = Convert.ToInt32(cmd.ExecuteNonQuery()) != 1 ? -1 : 0;


                    //第四句sql语句
                    stb.Clear();
                    stb.AppendFormat(" select scoretotal from Users where ID={0}",userid);
                    cmd.CommandText = stb.ToString();
                    int scoretotal = Convert.ToInt32(cmd.ExecuteScalar());

                    //第五句sql语句
                    stb.Clear();
                    stb.AppendFormat(" select ID from UserRoles where");
                    stb.AppendFormat(" {0} between minScore and maxScore",scoretotal);
                    cmd.CommandText = stb.ToString();
                    int userroleId = Convert.ToInt32(cmd.ExecuteScalar());

                    //第六句sql语句
                    stb.Clear();
                    stb.AppendFormat(" update Users set UserRoleId={0} where ID={1}",userroleId,userid);
                    cmd.CommandText = stb.ToString();
                    ret = Convert.ToInt32(cmd.ExecuteNonQuery()) != 1 ? -1 : 0;

                    tran.Commit();
                    return ret;
                }
                catch (Exception ex)
                {
                    ret = -1;
                    //有异常
                    tran.Rollback();//回滚事务
                    return ret;
                    throw ex;//跑出异常，由外界捕获再处理                    
                }          
            }
    */
        }
    }
