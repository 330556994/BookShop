using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using System.Data;
using System.Data.Entity;//实体框架命名空间


namespace Accp.Tools
{
    /// <summary>
    /// 这个是工具类，这里的方法都是扩展方法
    /// 扩展方法定义要求：
    /// 1.扩展方法必须定义在静态类
    /// 2.扩展方法都是静态方法
    /// 3.扩展方法第一个参数是 this 开头
    /// </summary>
    public static class ExtFunctions
    {
        /// <summary>
        /// 这是一个扩展方法，必须是静态
        /// </summary>
        /// <param name="str">这个方法是给类型string 注入,str代表那个对象</param>
        /// <param name="word">这才是本方法真正的参数了</param>
        /// <returns></returns>
        public static string Hello(this string str, string word) {
            return str + "：" + word;
        }
        /// <summary>
        /// 这是扩展方法，生成一个提交按钮
        /// </summary>
        /// <param name="helper">给htmlhelper类型注入</param>
        /// <param name="name">生成的Id和name属性值</param>
        /// <param name="value">value属性值</param>
        /// <returns></returns>
        public static MvcHtmlString Submit(this HtmlHelper helper, string name,string value) { 
           // <input type='submit' name="btn",id="btn" value="提交" />
            var builder = new TagBuilder("input");//创建input标签
            builder.MergeAttribute("type", "submit");//创建type属性并赋值
            builder.MergeAttribute("value", value);//创建value属性并赋值
            builder.MergeAttribute("name", name);//创建name属性并赋值
            builder.GenerateId(name);//创建id属性，并赋值
            //使用指定文本创建html字符串
            return MvcHtmlString.Create( builder.ToString());        
        }
        /// <summary>
        /// 这是扩展方法，生成一个提交按钮
        /// </summary>
        /// <param name="helper">给htmlhelper类型注入</param>
        /// <param name="name">生成的Id和name属性值</param>
        /// <param name="value">value属性值</param>
        /// <param name="htmlAttributes">注入额外html属性</param>
        /// <returns></returns>
        public static MvcHtmlString Submit(this HtmlHelper helper, 
            string name, string value,object htmlAttributes)
        {
                var builder = new TagBuilder("input");//创建input标签
            builder.MergeAttribute("type", "submit");//创建type属性并赋值
            builder.MergeAttribute("value", value);//创建value属性并赋值
            builder.MergeAttribute("name", name);//创建name属性并赋值
            builder.GenerateId(name);//创建id属性，并赋值
            //合并额外的html属性
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            //使用指定文本创建html字符串
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper,
            string name, object htmlAttributes, SelectList list,int rows=1,int cols=1) {
                TagBuilder builder = new TagBuilder("table");
                builder.GenerateId(name);
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                System.Text.StringBuilder html = new System.Text.StringBuilder();
                foreach (var item in list) { 
                    html.Append("\n\t<tr>");
                    html.Append("\n\t\t<td>");
                    html.Append(
     string.Format("\n\t\t\t<input type='checkbox' value='{0}' /><span>{1}</span>"
            , item.Value, item.Text
                        ));
                    html.Append("\n\t\t</td>");
                    html.Append("\n\t</tr>");
                }
                builder.InnerHtml = html.ToString();
                return MvcHtmlString.Create(builder.ToString());
        
        }

        public static MvcHtmlString EasyUiActionLink(this HtmlHelper helper
            ,string linkText,string actionName,
            string controlName,
            string name,
            object htmlAttributes){
        //<a href="/a/b" class="easyui-linkbutton"           //   >回到列表页</a>
                TagBuilder builder = new TagBuilder("a");
                builder.GenerateId(name);
                builder.InnerHtml = linkText;
                builder.MergeAttribute("class", "easyui-linkbutton");
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                string href = string.Format("/{0}/{1}", controlName, actionName);
                builder.MergeAttribute("href", href);
                return MvcHtmlString.Create(builder.ToString());


        }
        /// <summary>
        /// 这是扩展的泛型方法，
        /// 功能完成集合过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> MyWhere<T>
            (this List<T> list, BookShop.Models.MyFunc<T> func) {
                List<T> newList = new List<T>();
                foreach (T t in list) {
                    if (func(t)==true) {
                        newList.Add(t);
                    }
                }
                return newList;
        
        }
        /// <summary>
        /// 执行SQL语句，返回首行首列，为DbContext注入
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object   ExecuteScalar(this DbContext db,string   sql){
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = sql;
            
                db.Database.Connection.Open();
                return cmd.ExecuteScalar();
            
          
            
        }
        
       

    }
}