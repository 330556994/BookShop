using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 购物项类，用在购物下订单逻辑里
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 图书编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 书名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 会员价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; }

        
        /// <summary>
        /// 只读属性TotalUnitPrice 代表是 会员总价
        /// </summary>
        public decimal TotalUnitPrice { 
            get {
                return this.Qty * this.UnitPrice;
        } }
        /// <summary>
        /// 市场总价
        /// </summary>
        public decimal TotalMarketPrice {
            get {
                return this.Qty * this.MarketPrice;
            }
        }
    }
}