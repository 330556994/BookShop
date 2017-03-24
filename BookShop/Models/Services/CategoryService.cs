using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BookShop.Models.Entities;
using System.Data;
namespace BookShop.Models.Services
{
    /// <summary>
    /// 图书类别管理类
    /// </summary>
    public class CategoryService
    {
        public List<Category> GetList()
        {
            //从当前上下文的缓存里得到数据
            List<Category> list = HttpContext.Current.Cache["category"] 
                              as List<Category>;
            if (list == null) { 
            //没有获得，则从数据库中读取
                //select * from Categories order by SortNum asc 
                string sql = "select * from Categories order by SortNum asc ";//定义查询语句
                var ds = Accp.Tools.DbSqlHelper.Query(sql);
                list = new List<Category>();
                Category c = null;
                //循环遍历所有行
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    c = new Category();
                    c.Id = Convert.ToInt32(row["id"]);//取出row行的id列转换成int赋值给对象的属性
                    //如果Name列是允许为空的话，必须要做判断，再赋值
                    //当数据表中name列是 not null 约束
                    if (row.IsNull("Name") == false)
                    {
                        //检测name列是否有值，如果有值为false
                        c.Name = Convert.ToString(row["Name"]);//有值才赋值

                    }
                    c.SortNum = Convert.ToInt32(row["SortNum"]);
                    list.Add(c);//将这个对象添加到集合里
                }
                //HttpContext.Current.Cache.Insert("category", list);
                //创建一个缓存对象，把这个集合添加到缓存里
                HttpContext.Current.Cache["category"] = list;
            }

            
            return list;//返回泛型集合


        }
        /// <summary>
        /// 返回图书类别集合带类别下图书总数，本方法在books/index中会调用
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListWithBooCount()
        {
            //select * from Categories order by SortNum asc 
            string sql = "select * from Categories order by SortNum asc ";//定义查询语句
            var ds = Accp.Tools.DbSqlHelper.Query(sql);
            BookService bookservice = new BookService();
            List<Category> list = new List<Category>();
            Category c = null;
            //循环遍历所有行
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                c = new Category();
                
                c.Id = Convert.ToInt32(row["id"]);//取出row行的id列转换成int赋值给对象的属性
                //获得该类别下图书总数
                c.BookCount = bookservice.GetRecordCount(c.Id);
                //如果Name列是允许为空的话，必须要做判断，再赋值
                //当数据表中name列是 not null 约束
                if (row.IsNull("Name") == false)
                {
                    //检测name列是否有值，如果有值为false
                    c.Name = Convert.ToString(row["Name"]);//有值才赋值

                }
                c.SortNum = Convert.ToInt32(row["SortNum"]);
                list.Add(c);//将这个对象添加到集合里
            }
            return list;//返回泛型集合


        }

        /// <summary>
        /// 根据类别编号获得单个类别对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns>存在id则返回对象，不存在返回null</returns>
        public Category GetSingle(int id)
        {
            //select * from Categories where ID=25
            string sql = string.Format("select * from Categories where ID={0}", id);
            Category c = null;
            var ds = Accp.Tools.DbSqlHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                c = new Category();
                c.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
                c.Name = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);
                c.SortNum = Convert.ToInt32(ds.Tables[0].Rows[0]["SortNum"]);
            }
            return c;

        }
 

    }
}