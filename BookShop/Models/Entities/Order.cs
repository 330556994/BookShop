using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 订单实体类
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public int Id { get; set; }  
        /// <summary>
        /// 订购日期
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 所属会员编号
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 订购商品总价
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 订单状态 1:未处理  只有这个状态会员可以取消
        /// 2处理中   
        /// 3.已发货 4.会员取消订单
        /// 5.管理员取消订单 6.已收货  7.已完成  
        /// 
        ///8.订单已取消
        ///
        /// 
        /// ////////商讨中   ：9 申请退货中  10：管理员确认退货 11 我方已收货  12:退货完成  12：
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string    PersonName { get; set; }
        /// <summary>
        /// 收件人电话号码
        /// </summary>
        public string    Phone { get; set; }
        /// <summary>
        /// 送件地址
        /// </summary>
        public string    Address { get; set; }
        /// <summary>
        /// 快递费用
        /// </summary>
        public int    SendCash { get; set; }
        /// <summary>
        /// 会员取消理由，当会员取消订单时，必须输入取消理由
        /// </summary>
        public string    UserCancelReason { get; set; }
        /// <summary>
        /// 订单备注，会员填写
        /// </summary>
        public string OrderRemark { get; set; }
        ///////////////////////////////////////////////////////////////////////////
        // 因为业务模型中，通常是通过订单看明细
        // 所以，这里的一对多的设计，采用的是主表里建明细的集合
        //////////////
        /// <summary>
        /// 该订单的明细集合
        /// </summary>
        public List<OrderBook> OrderBooks { get; set; }

    }
}