using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accp.Orm;//这是自己写的ORM框架命名空间
namespace BookShop.Models
{
    /// <summary>
    /// 公共汽车实体类
    ///  数据表 tbBus (busId,name,company)
    /// </summary>
    [Table(TableName="tbBus")] //隐射表名
    public class Bus
    {
        /// <summary>
        /// 编号，使用自定义特性类来隐射列名
        /// </summary>
       [Field(FieldName="busId",IsPrimayKey=true)]
        public int Id { get; set; }
        /// <summary>
        /// 汽车名称
        /// </summary>
        [Field( FieldName="name")]
        public string Name { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [Field(FieldName = "company")]
        public string Supply { get; set; }
    }
}