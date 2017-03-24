using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accp.Tools;

using BookShop.Models.Entities;

namespace BookShop.Models.Services
{
    public class UserService
    {
        /// <summary>
        /// 判断用户名和密码是否正确
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>正确返回真，否则假</returns>
        public bool Login(string name, string pwd) {
            //select COUNT(*) from UserS where LoginId='bobo'
 //and LoginPwd='123456'
            int isFrozen=-1;    
            string sql = string.Format("select COUNT(*) from UserS"
                 + " where LoginId='{0}' and LoginPwd='{1}' "
                 , name, pwd
                );
            int ret = Convert.ToInt32(DbSqlHelper.ExecuteScalar(sql));
            if(ret==1){
                isFrozen=Convert.ToInt32(DbSqlHelper.ExecuteScalar(string.Format("select IsFrozen from users where LoginId='{0}'",name)));
            }
            return isFrozen==0? true : false;
        
        }
        /// <summary>
        /// 判断用户是否已经登录网站，如未登录，则跳转到登录页面
        /// </summary>
        public void CheckUser() {
            //取出保持在session里的变量，名字叫name的值
            //在model里是不能直接访问request,response,session,cookie,application
            // server，这些对象需要请求上下文
            //HttpContext.Current
            if (HttpContext.Current. Session["name"] == null)
            {
                string html = string.Format("<script>alert('必须先登录才能访问');"
                + "location.href='/user/login?returnurl={0}';</script>"
                , HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"]);

                HttpContext.Current.Response.Write(html);//写入客户端
                HttpContext.Current.Response.End();//结束请求，发送响应流到客户端
            }
        
        }

        /// <summary>
        /// 根据会员编号，获得该会员的订单地址信息
        /// 业务要求：如果该会员没有下过订单，则地址信息到会员表中取
        /// 下过订单，则地址信息是该会员最近下的订单地址信息
        /// </summary>
        /// <param name="id">会员编号,不是会员账号</param>
        /// <returns>地址信息</returns>
        public OrderAddress GetOrderAddressByUserId(int userId) {
         //思路： 先创建一个地址信息对象
            //写sql从订单中读取编号为id的会员下过的最近一条订单
           //判断是否得到一条，如果没有得到，那么写sql读取该会员表里的记录
            //将表中的数据赋值给地址信息对象
            //如果该会员下过订单，则地址信息从订单表中获取
            //最后返回已赋值的地址对象
            //select top 1  phone,address,personName
 //from orders where UserId=3 order by OrderDate desc
            OrderAddress orderAddress = new OrderAddress();
            string sql = string.Format("select top 1  phone,address,personName"
                + " from orders where UserId={0} "
                + "  order by OrderDate desc"
                , userId
                );
            var ds = DbSqlHelper.Query(sql);
            if (ds.Tables[0].Rows.Count == 1)
            {
                //存在
                orderAddress.Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                orderAddress.Name = Convert.ToString(ds.Tables[0].Rows[0]["personName"]);
                orderAddress.Phone = Convert.ToString(ds.Tables[0].Rows[0]["phone"]);
            }
            else { 
            //不存在
                sql = "select Name,Address,Phone from Users where ID=" + userId;
                ds = DbSqlHelper.Query(sql);
                orderAddress.Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                orderAddress.Name = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);
                orderAddress.Phone = Convert.ToString(ds.Tables[0].Rows[0]["phone"]);
         
            }
            return orderAddress;


        }

        /// <summary>
        /// 根据会员登录账号，获得该会员编号
        /// 调用：在下订单的时候，会调用这个方法
        /// </summary>
        /// <param name="loginid"></param>
        /// <returns></returns>
        public int GetUserId(string loginid) {
            //select id from Users where LoginId='bobo' 
            string sql = string.Format("select id from Users where LoginId='{0}' ", loginid);
            return Convert.ToInt32(DbSqlHelper.ExecuteScalar(sql));
        
        }
    }
}