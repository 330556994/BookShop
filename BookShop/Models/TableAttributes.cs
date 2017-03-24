using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accp.Orm
{
    /// <summary>
    /// 定义一个自定义特性类，用来修饰类。这里处理，类名和表名的映射关系
    /// 一般命名规则，特性类都是以Attribute结尾
    /// </summary>
    public class TableAttribute:Attribute
    {
        /// <summary>
        /// 这个属性表示表的名字是什么
        /// </summary>
        public string TableName { get; set; }
    }
}