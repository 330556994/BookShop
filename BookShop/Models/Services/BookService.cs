using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShop.Models.Entities;
using System.Data;
using Accp.Tools;
using System.Data.SqlClient;
namespace BookShop.Models.Services
{
    /// <summary>
    /// 图书业务类
    /// </summary>
    public class BookService
    {
        /// <summary>
        /// 得到某个图书类别下，图书总数
        /// </summary>
        /// <param name="categoryid">类别编号</param>
        /// <returns>图书总数</returns>
        public int GetRecordCount(int categoryid) {
            //select count(id) from Books where CategoryId=25
            string sql = string.Format("select count(id) from Books where CategoryId={0}", categoryid);
            return Convert.ToInt32(DbSqlHelper.ExecuteScalar(sql));
        }

        /// <summary>
        /// 首页显示新书排行榜，根据出版日期倒序，只返回id,title列
        /// </summary>
        /// <param name="top">要取多少本书</param>
        /// <returns>图书集合，只有id，title有内容</returns>
        public List<Book> GetNewBooks(int top) {

            string sql = string.Format(" select top {0} id,title from books order by publishdate desc",top);
            var ds = DbSqlHelper.Query(sql);
            List<Book> list = new List<Book>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Book book = new Book();
                book.Id = Convert.ToInt32(row["id"]);
                book.Title = Convert.ToString(row["Title"]);
                list.Add(book);
            }
            return list;
          
        }
        /// <summary>
        /// 首页显示热门排行榜，根据clicks倒序取数据，只返回id.title
        /// </summary>
        /// <param name="top">取多少本书</param>
        /// <returns>图书集合</returns>
        public List<Book> GetHotBooks(int top)
        {
            string sql = string.Format(" select top {0} id,title from books order by clicks desc", top);
            var ds = DbSqlHelper.Query(sql);
            List<Book> list = new List<Book>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Book book = new Book();
                book.Id = Convert.ToInt32(row["id"]);
                book.Title = Convert.ToString(row["Title"]);
                list.Add(book);
            }
            return list;
        }
        /// <summary>
        /// 得到首页里的图书内容，首页调用
        /// </summary>
        /// <param name="top">取多少条</param>
        /// <returns></returns>
        public List<Book> GetHomeBooks(int top)
        {
            string sql = string.Format(" select top {0} id,title,isbn from books ", top);
            var ds = DbSqlHelper.Query(sql);
            List<Book> list = new List<Book>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Book book = new Book();
                book.Id = Convert.ToInt32(row["id"]);
                book.Title = Convert.ToString(row["Title"]);
                book.ISBN = Convert.ToString(row["ISBN"]);
                list.Add(book);
            }
            return list;
        }

        /// <summary>
        /// 获取全部图书信息
        /// </summary>
        /// <returns>图书集合</returns>
        public List<Book> GetList() {
            /*
             * select a.*,b.Name as  Cname,b.SortNum,
c.Name as Pname
 from books a 
  inner join Categories b
  on a.CategoryId=b.Id
  inner join Publishers c
  on a.PublisherId=c.Id
  */
            string sql = "select a.*,b.Name as  Cname,b.SortNum,"
                  + " c.Name as Pname"
                  + " from books a "
                  + "  inner join Categories b"
                  + " on a.CategoryId=b.Id"
                  + " inner join Publishers c"
                  + "  on a.PublisherId=c.Id";
            DataSet ds = DbSqlHelper.Query(sql);
            return ConvertDataSetToList(ds);//调用内部方法，转换成泛型集合
        }
        /// <summary>
        /// 根据图书类别编号，获得该类别下图书信息
        /// </summary>
        /// <param name="categoryid">图书类别编号</param>
        /// <returns>图书集合</returns>
        public List<Book> GetList(int categoryid) {
            /*
select a.*,b.Name as  Cname,b.SortNum,
c.Name as Pname
 from books a 
  inner join Categories b
  on a.CategoryId=b.Id
  inner join Publishers c
  on a.PublisherId=c.Id
  where CategoryId=25  */
            System.Text.StringBuilder  sql =new System.Text.StringBuilder("select a.*,b.Name as  Cname,b.SortNum,"
                  + " c.Name as Pname"
                  + " from books a "
                  + "  inner join Categories b"
                  + " on a.CategoryId=b.Id"
                  + " inner join Publishers c"
                  + " on a.PublisherId=c.Id"
                  +" where 1=1 ");
            if (categoryid != 0) { 
                //如果有类别查询条件，则需要拼查询条件
                sql.Append(" and categoryid=" + categoryid);
            }
            DataSet ds = DbSqlHelper.Query(sql.ToString());
            return ConvertDataSetToList(ds);           
        }
        /// <summary>
        /// 带分页和排序功能的查询图书，用在图书列表页中
        /// sql已经创建了该功能的存储过程
        /// proc_getbooks 10,30,'a.id desc',25
        /// </summary>
        /// <param name="start">从第几条</param>
        /// <param name="end">到第几条</param>
        /// <param name="sort">排序条件如 a.id desc</param>
        /// <param name="categoryid">图书类别编号</param>
        /// <returns>返回图书泛型集合</returns>
        public List<Book> GetList(int start, int end, string sort, int categoryid) { 
        // 这里演示如何调用返回结果集的存储过程
            using (SqlConnection conn = 
                new SqlConnection(DbSqlHelper.connstr)) {
                    conn.Open();//打开链接
                    SqlCommand cmd = conn.CreateCommand();//创建命令
                //创建输入参数
                    SqlParameter p_start = new SqlParameter("@start", start);
                    p_start.SqlDbType = SqlDbType.Int;//设置参数类型
                //默认就是输入参数
                    p_start.Direction = ParameterDirection.Input;//表示输入参数
                    SqlParameter p_end = new SqlParameter("@end", end);
                    p_end.SqlDbType = SqlDbType.Int;//设置参数类型
                    SqlParameter p_sort = new SqlParameter("@sort", sort);
                    p_sort.SqlDbType = SqlDbType.VarChar;//设置参数类型
                    p_sort.Size = 50;//设置字符varchar类型长度
                    SqlParameter p_categoryid = 
                        new SqlParameter("@categoryid", categoryid);
                    p_categoryid.SqlDbType = SqlDbType.Int;//设置参数类型
                //按照存储过程定义参数的顺序，添加参数
                    cmd.Parameters.Add(p_start);
                    cmd.Parameters.Add(p_end);
                    cmd.Parameters.Add(p_sort);
                    cmd.Parameters.Add(p_categoryid);
                //设置cmd调用格式为存储过程
                    cmd.CommandType = CommandType.StoredProcedure;
                //设置调用存储过程名字
                    cmd.CommandText = "proc_getbooks";
               
                    //cmd.ExecuteNonQuery(); 如果不返回结果集，就这样调用
                //p_sort.Value 检索某个输出参数值
                  //这个存储过程是返回结果集的
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);//填充结果集
                    return ConvertDataSetToList(ds);   

            
            }
        }
        /// <summary>
        /// 根据图书编号，获得单个图书对象
        /// </summary>
        /// <param name="id">类别编号</param>
        /// <returns>图书对象</returns>
        public Book GetSingle(int id) {
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
                  + " where a.id=" + id ;
            DataSet ds = DbSqlHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //如果有记录
                var list = ConvertDataSetToList(ds);
                return list[0];
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// 私有方法，将一个dataset转换成泛型集合
        /// </summary>
        /// <param name="ds">要转换的dataset</param>
        /// <returns>图书泛型集合</returns>
        private List<Book> ConvertDataSetToList(DataSet ds) {
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
        private decimal GetBookRating(int bookid) { 
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
            catch (Exception ex) {
                ret = 0;
            }
            //先得到整数部分
            decimal  number = Math.Floor(ret);
            //得到转换后的评分，超过0.5则半星，否则就1星
            decimal retNumber = ret > (number + Convert.ToDecimal(0.5))
                ? number + Convert.ToDecimal(0.5) : number;

            return retNumber;
        }

    }
}