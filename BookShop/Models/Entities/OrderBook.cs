using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 订单明细类
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// 明细编号
        /// </summary>
       public int Id { get; set; } 
        /// <summary>
        /// 所属订单编号
        /// </summary>
       public int OrderID { get; set; } 
        /// <summary>
        /// 购买图书编号
        /// </summary>
       public int BookID { get; set; } 
        /// <summary>
        /// 购买数量
        /// </summary>
       public int Quantity { get; set; }
        /// <summary>
        /// 图书单价
        /// </summary>
       public decimal UnitPrice { get; set; } 




    }
}