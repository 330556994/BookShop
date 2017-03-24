using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 出版社实体类
    /// </summary>
    public class Publisher
    {
        /// <summary>
        /// 出版社编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 出版社名称
        /// </summary>
        public string Name { get; set; }
    }
}