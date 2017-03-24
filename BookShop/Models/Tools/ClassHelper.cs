using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
namespace Accp.Tools
{
    /// <summary>
    /// 这是一个类助手
    /// </summary>
    public class ClassHelper
    {
        /// <summary>
        /// 本方法功能，将传入的datatable转换成泛型集合
        /// 前提：必须T类型中的属性名要和数据表中的列名一致
        /// </summary>
        /// <typeparam name="T">是泛型，需要转的类型</typeparam>
        /// <param name="dt">数据表</param>
        /// <returns>转换后的泛型集合</returns>
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
             where T:new() //where是约束 表示T这个类型必须能够实例化
        {
            List<T> list = new List<T>();
            Type type = typeof(T);//typeof是运算符，得到一个类型的Type实例
            
            foreach (DataRow row in dt.Rows) {
                T t = new T();//实例化一个泛型对象
                foreach (DataColumn col in dt.Columns) { 
                    //循环该行里的所有列
                    //因为列名就是类的属性名,根据列名获得类型的属性对象
                    var prop = type.GetProperty(col.ColumnName);
                    //给属性赋值,付列的值
                    prop.SetValue(t, row[col]);
                }
                list.Add(t);
            }
            return list;
        }

        //将json格式转换成对象
        public static T ConvertJsonToObject<T>(string json) where
          T:new () 
        { 
            // {"id":1,"name":"张三","address":"上海"};
            //先去除大括号，和双引号
            json = json.Replace("{", "").Replace("}", "").Replace("\"", "");
            string[] parts = json.Split(',');//按照逗号分隔
            Type type = typeof(T);
            T t = new T();
            foreach (string str in parts) {
                var sections = str.Split(':');//变成两部分，名字 值
                PropertyInfo p = type.GetProperty(sections[0]);
                if (p != null)
                {
                    string typestr = p.PropertyType.ToString();
                    if (typestr == "System.Int32")
                    {
                          p.SetValue(t, Convert.ToInt32(sections[1]));
                    }
                    else {
                        p.SetValue(t,sections[1]);
                    }
                    
                }
            }
            return t;
        }
        //约定，实体类的类名就是表名，属性名就是列名
        public static string GetInsertSql(object obj,string primaykey) {
            //生成不含主键字段的sql语句
            //表名
            Type type = obj.GetType();//获得对象类型
            string tablename = type.Name;
            var props = type.GetProperties();//获得所有属性
            string cols = "";
            string vals = "";
            foreach (var p in props) { 
            //循环变量属性
                //p.Name //属性名
                string val = Convert.ToString(p.GetValue(obj));//获得该属性值
                if (p.Name != primaykey)
                {
                    cols += p.Name + ",";
                    vals += "'" + val + "',";
                }
            }
            //去除两个逗号，完成拼接
            vals = vals.Substring(0, vals.Length - 1);
            cols = cols.Substring(0, cols.Length - 1);
            string sql=String.Format("insert into {0} ({1}) values({2})"
                , tablename, cols, vals
                );
            return sql;
            //return "insert into publishers (name) values('北大青鸟')";
        }




    }
}








