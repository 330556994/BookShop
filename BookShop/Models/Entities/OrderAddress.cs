using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 订单地址类，描述一个订单的收货地址信息
    /// </summary>
    public class OrderAddress
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 送货地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string Name { get; set; }
    }
}