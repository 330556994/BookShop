using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BookShop.Models.Entities
{
    /// <summary>
    /// 学生实体类，业务模型，靠近业务的
    /// </summary>
    // partial 修饰类表示：局部类，即一个类可以定义在多个文件里
    public partial class Student2
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
     
        public int Score { get; set; }
        public DateTime? Birthday { get; set; }
        public string Description { get; set; }
      
        public int ClassId { get; set; }
        /// <summary>
        /// 学生头像，这里只保存文件名，保存路径在/upload目录下
        /// </summary>
        public string ImgFile { get; set; }

    }
}