using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//下面这两个命名空间是验证框架的
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.Entities
{
    /// <summary>
    /// 学生实体类，为了演示验证框架使用
    /// 常见的表单验证有哪些？
    /// 1.必填校验  2.两次是否相同 3.长度校验  4.正则校验，如电子邮件
    /// 5. 类型检查 必须输入数字  6. 范围校验，如成绩0-100之间
    /// 有的验证必须服务端处理：帐号不能重复
    /// 一般表单校验：js的客户端校验，服务端还要校验
    /// </summary>
    [MetadataType(typeof(Student2MetaData))]
    public partial class Student2
    {
        //下面这两个属性，是属于视图模型的，所以，放在外面
        /// <summary>
        /// 重复密码，注意这个属性数据表里没有的，这里扩展一个的目的是为了界面视图用
        /// </summary>
        [DisplayName("重复密码")]
        //这是比较校验，第一个参数表示和哪个属性比较
        [Compare("Pwd", ErrorMessage = "两次输入密码不一致")]
        [DataType(DataType.Password)]
        public string PwdAgain { get; set; }
        /// <summary>
        /// 验证码，这个属性是扩展的，为了视图中使用验证码验证
        /// </summary>
        [DisplayName("验证码")]
        [Required(ErrorMessage = "必须输入验证码")]
        public string Code { get; set; }

        /// <summary>
        /// 这叫内部类，这里将属于视图模型类的验证特性加在这里
        /// </summary>
        public class Student2MetaData {

          
            /// <summary>
            /// 学生姓名
            /// </summary>
            [DisplayName("学生姓名")] //这个特性是显示提示信息用
            [Required(ErrorMessage = "姓名必须输入")] //必填特性
            //长度校验，{0}代表的是输入值
            [StringLength(20, MinimumLength = 6, ErrorMessage = "{0}长度必须介于{1},{2}之间")]
            public string Name { get; set; }
            /// <summary>
            /// 密码
            /// </summary>
            [DisplayName("密码")] //这个特性是显示提示信息用
            [Required(ErrorMessage = "密码必须输入")] //必填特性
            //这个特性是指 数据类型，当视图使用Editfor方法来生成表单元素时
            //Editfor方法会根据加在属性上的这个特性来自动生成对应的html标签
            [DataType(DataType.Password)]
            //密码必须要6-12位之间，这就需要正则表达式校验
            [RegularExpression(@"\w{6,12}", ErrorMessage = "密码格式错误，必须是6-12个字符")]
            public string Pwd { get; set; }
   
            /// <summary>
            /// 成绩  int ,int?
            /// 这个是int是必须要有值的，所以，自动会生成必填校验
            /// 成绩0-100 范围验证
            /// </summary>
            [DisplayName("成绩")]
            [Range(0, 100, ErrorMessage = "成绩必须介于{0}-{1}之间")]
            public int Score { get; set; }
            /// <summary>
            /// 出生年月
            /// </summary>
            [DisplayName("出生年月")]
            [DataType(DataType.Date, ErrorMessage = "日期输入错误")]
            public DateTime? Birthday { get; set; }
            /// <summary>
            /// 学生简介
            /// </summary>
            [DisplayName("学生简介")]
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }
        
            /// <summary>
            /// 班级编号
            /// </summary>
            /// 
            [DisplayName("选择班级")]
            public int ClassId { get; set; }

            [DisplayName("学生头像")]            
            public string ImgFile { get; set; }


        }

    }
}