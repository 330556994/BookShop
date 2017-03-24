using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using BookShop.Models.Entities;

namespace BookShop.Areas.Admin.Models.Services
{
    public class CategoryService
    {
        public List<Category> GetList()
        {
            //select * from Categories order by SortNum asc 
            string sql = "select * from Categories order by SortNum asc ";//定义查询语句
            var ds = Accp.Tools.DbSqlHelper.Query(sql);
            List<Category> list = new List<Category>();
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
            return list;//返回泛型集合


        }

    }
}