using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accp.Orm
{
    /// <summary>
    /// 自定义特性类，用来描述字段和属性的映射关系
    /// </summary>
    public class FieldAttribute:Attribute
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimayKey { get; set; }


    }
}