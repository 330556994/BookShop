using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models.Entities
{
    public class Book
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
        /// ISBN   1 a-f 2[0-5] 12个数字 最后一个是字母
         /// </summary>
        public string ISBN { get; set; }
        
        /// <summary>
        ///作者 
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 出版社编号
        /// </summary>
        public int PublisherId { get; set; }
        /// <summary>
        /// 出版日期
        /// </summary>
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// vip价额
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 内容描述
        /// </summary>
        public string ContentDescription { get; set; }
        /// <summary>
        ///章节描述 
        /// </summary>
        public string TOC { get; set; }
         /// <summary>
        ///  //图书类别编号
         /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// //点击率
        /// </summary>
        public int Clicks { get; set; }
        /// <summary>
        ///类别和图书是一对多关系 
        /// </summary>
        public Category Category { get; set; }
        /// <summary>
        /// 出版社和图书室一对多关系
        /// </summary>
        public Publisher Publisher { get; set; }
        /// <summary>
        /// 这个属性是扩展的，数据库里不存在，表示图书评分
       /// </summary>
        public decimal BookRating { get; set; }

    }
}