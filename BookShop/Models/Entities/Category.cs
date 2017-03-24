using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 图书类别实体类
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 类别编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序字段，显示列表时，默认的排序方式
        /// </summary>
        public int SortNum { get; set; }
        /// <summary>
        /// 该类别下图书总数，这个属性是为了在前台列表页中用
        /// </summary>
        public int BookCount { get; set; }
 
    }
}