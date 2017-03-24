using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//下面这两个命名空间是验证框架的
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Areas.Admin.Models
{
    
    [MetadataType(typeof(sysmanMetaData))]
    public partial class sysman
    {
        public class sysmanMetaData
        {
            [DisplayName("会员编号")]
            public int id { get; set; }
            [DisplayName("用户名")]
            [Required(ErrorMessage = "用户名必须输入")] //必填特性
            public string name { get; set; }
            [DisplayName("密码")] 
            [Required(ErrorMessage = "密码必须输入")] //必填特性
            [DataType(DataType.Password)]        
            public string pwd { get; set; }
        }
    }
}

