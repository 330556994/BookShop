using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShop.Models.Entities;
using System.Data;
using System.Data.SqlClient;
using Accp.Tools;
namespace BookShop.Areas.Admin.Models.Services
{
    public class BookService
    {
        /// <summary>
        /// 得到某个图书类别下，图书总数
        /// </summary>
        /// <param name="categoryid">类别编号</param>
        /// <returns>图书总数</returns>
        public int GetRecordCount(int categoryid,string title)
        {
            //先构造where条件
            System.Text.StringBuilder where = new System.Text.StringBuilder("");
            //有类别
            if (categoryid > 0)
            {
                where.Append(string.Format(" and  CategoryId={0} ", categoryid));
            }
            //检查是否需要拼接书名查询条件
            if (string.IsNullOrEmpty(title) == false)
            {
                //有值拼sql
                where.Append(string.Format(" and Title like '%{0}%'", title));
            }
            string sql = string.Format("select count(id) from Books where 1=1 {0}", where.ToString());
            return Convert.ToInt32(DbSqlHelper.ExecuteScalar(sql));
        }

        /// <summary>
        /// 分页功能图书查询方法
        /// </summary>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="categoryid">图书类别</param>
        /// <param name="title">书名</param>
        /// <returns></returns>
        public List<Book> GetList(int start, int end, 
            int categoryid,string title)
        {
            /*
             * select * from (
select ROW_NUMBER() over(order by a.id asc 
) as rowid,
 a.*,b.Name as  Cname,b.SortNum,
c.Name as Pname
 from books a 
  inner join Categories b
  on a.CategoryId=b.Id
  inner join Publishers c
  on a.PublisherId=c.Id
  where 1=1 and  CategoryId=25 and Title like '%asp.net%') tablea
    where rowid between 10 and 20
          */
            //先构造where条件
            System.Text.StringBuilder where = new System.Text.StringBuilder("");
            //有类别
            if (categoryid > 0) {            
                where.Append(string.Format(" and  CategoryId={0} ", categoryid));
            }
            //检查是否需要拼接书名查询条件
            if (string.IsNullOrEmpty(title) == false) { 
                //有值拼sql
                where.Append(string.Format(" and Title like '%{0}%'", title));
            }
            //构造完整的查询语句
            string sql = string.Format("select * from ( "
                        + "select ROW_NUMBER() over(order by a.id asc "
                        + " ) as rowid,"
                        + " a.*,b.Name as  Cname,b.SortNum,"
                        + " c.Name as Pname "
                        + "  from books a "
                        + "   inner join Categories b "
                        + "   on a.CategoryId=b.Id "
                        + "   inner join Publishers c "
                        + "   on a.PublisherId=c.Id "
                        + "   where 1=1 {0}) tablea "
                        + "    where rowid between {1} and {2}",
                        where.ToString(), start, end
                        );
            DataSet ds = DbSqlHelper.Query(sql);
            return ConvertDataSetToList(ds);

        }
        /// <summary>
        /// 根据图书编号，获得单个图书对象
        /// </summary>
        /// <param name="id">类别编号</param>
        /// <returns>图书对象</returns>
        public Book GetSingle(int id)
        {
            /*select a.*,b.Name as  Cname,b.SortNum,
    c.Name as Pname
     from books a 
      inner join Categories b
      on a.CategoryId=b.Id
      inner join Publishers c
      on a.PublisherId=c.Id
      where a.id=4946
             * */
            string sql = "select a.*,b.Name as  Cname,b.SortNum,"
                  + " c.Name as Pname"
                  + " from books a "
                  + "  inner join Categories b"
                  + " on a.CategoryId=b.Id"
                  + " inner join Publishers c"
                  + "  on a.PublisherId=c.Id"
                  + " where a.id=" + id;
            DataSet ds = DbSqlHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //如果有记录
                var list = ConvertDataSetToList(ds);
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 私有方法，将一个dataset转换成泛型集合
        /// </summary>
        /// <param name="ds">要转换的dataset</param>
        /// <returns>图书泛型集合</returns>
        private List<Book> ConvertDataSetToList(DataSet ds)
        {
            List<Book> list = new List<Book>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Book book = new Book();
                book.Id = Convert.ToInt32(row["id"]);
                book.ISBN = Convert.ToString(row["ISBN"]);
                book.Author = Convert.ToString(row["Author"]);
                book.CategoryId = Convert.ToInt32(row["CategoryId"]);
                book.Clicks = Convert.ToInt32(row["Clicks"]);
                if (row.IsNull("ContentDescription") == false)
                {
                    book.ContentDescription = Convert.ToString(row["ContentDescription"]);
                }
                if (row.IsNull("MarketPrice") == false)
                {
                    book.MarketPrice = Convert.ToDecimal(row["MarketPrice"]);
                }
                book.PublishDate = Convert.ToDateTime(row["PublishDate"]);
                book.PublisherId = Convert.ToInt32(row["PublisherId"]);
                book.Title = Convert.ToString(row["Title"]);
                //toc是允许为空的，必须要做判断再赋值
                if (row.IsNull("TOC") == false)
                {
                    book.TOC = Convert.ToString(row["TOC"]);
                }
                book.UnitPrice = Convert.ToDecimal(row["UnitPrice"]);
                //下面开始处理外键对象
                book.Publisher = new Publisher();
                book.Publisher.Id = book.PublisherId;
                book.Publisher.Name = Convert.ToString(row["Pname"]);
                book.Category = new Category();
                book.Category.Id = book.CategoryId;
                book.Category.Name = Convert.ToString(row["Cname"]);
                book.BookRating = GetBookRating(book.Id);//获得这本书的评分
                list.Add(book);
            }
            return list;
        }
        /// <summary>
        /// 得到一本书的评分
        /// 一本书的评分 >2.0  <2.5  2星， >2.5- <3 2星半
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        private decimal GetBookRating(int bookid)
        {
            //select (sum(Rating)*1.0)/COUNT(Rating)  as score
            //from BookRatings
            //where BookId=4946  
            string sql = string.Format("select (sum(Rating)*1.0)/COUNT(Rating)"
                + " as score from BookRatings"
                + " where BookId={0}  "
                , bookid
                );
            decimal ret;
            try
            {
                //如果这本书没有评分 dbnull,不能赋值给ret，抛异常
                ret = Convert.ToDecimal(DbSqlHelper.ExecuteScalar(sql));
            }
            catch (Exception ex)
            {
                ret = 0;
            }
            //先得到整数部分
            decimal number = Math.Floor(ret);
            //得到转换后的评分，超过0.5则半星，否则就1星
            decimal retNumber = ret > (number + Convert.ToDecimal(0.5))
                ? number + Convert.ToDecimal(0.5) : number;

            return retNumber;
        }

    }
}