using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accp.Orm
{
    /// <summary>
    /// 这是我们ORM框架的核心数据操作类
    /// </summary>
    public class DbContent
    {
        //最好这里自己内置dbsqlhelper中的所有功能
        /// <summary>
        /// 根据对象获得插入语句
        /// </summary>
        /// <param name="obj">要插入的对象，前提，本对象必须使用我们ORM框架里的特性</param>
        /// <returns></returns>
        public string GetInsertSql(object obj) {
            //生成不含主键字段的sql语句
            //表名
            Type type = obj.GetType();//获得对象类型
            //获得加在类上的自定义特性TableAttribute, false代表本类上，不去查继承
            var attrs = type.GetCustomAttributes(typeof(TableAttribute),false);
            //按照使用ORM的规则，加在类上的table特性只有一个
            TableAttribute tableAttr = attrs[0] as TableAttribute;//类型强转
            string tablename =tableAttr.TableName;//这是隐射的实际表名
            var props = type.GetProperties();//获得所有属性
            string cols = "";
            string vals = "";
            foreach (var p in props)
            {
                //循环变量属性
                //获得加在属性上的自定义特性FieldAttribute
                var attrsFields = p.GetCustomAttributes(typeof(FieldAttribute), false);
                //按照ORM使用规则，加在属性上的这个特性只能一个
                FieldAttribute fieldAttr = attrsFields[0] as FieldAttribute;
   
                string val = Convert.ToString(p.GetValue(obj));//获得该属性值
                if (fieldAttr.IsPrimayKey!=true)
                {
                    //如果该列不是主键列的话，则需要拼字符串
                    cols += fieldAttr.FieldName + ",";
                    vals += "'" + val + "',";
                }
            }
            //去除两个逗号，完成拼接
            vals = vals.Substring(0, vals.Length - 1);
            cols = cols.Substring(0, cols.Length - 1);
            string sql = String.Format("insert into {0} ({1}) values({2})"
                , tablename, cols, vals
                );
            return sql;
          
        
        }
    }
}