using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShop.Models.Entities;//导入实体类命名空间
using Accp.Tools; //工具包
using System.Data;
using System.Data.SqlClient;//数据命名空间
namespace BookShop.Models.Services
{
    /// <summary>
    /// 作者：金兵
    /// 功能描述： 出版社的业务类
    /// 创建日期：2016-9-23
    /// </summary>
    public class PublisherService
    {
        /// <summary>
        /// 得到出版社所有信息
        /// </summary>
        /// <returns>出版社集合</returns>
        public List<Publisher> GetList() {
            //select * from Publishers
            string sql = "select * from Publishers order by id ";
            DataSet ds = DbSqlHelper.Query(sql);
            List<Publisher> list = new List<Publisher>();
            foreach (DataRow row in ds.Tables[0].Rows) {
                Publisher p = new Publisher();
                p.Id = Convert.ToInt32(row["id"]);
                p.Name = Convert.ToString(row["name"]);
                list.Add(p);
            }
            return list;

        }
        /// <summary>
        /// 分页版本
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Publisher> GetList(int start, int end) { 
        /*@tablename varchar(50) ,--表名
	@fields varchar(100)='*',--要显示的列名，默认*，如 * 或  id,name,age 
	@start varchar(10) ='1',--默认是1 起始索引
	@end varchar(10) ='10', --默认是10 结束索引
	@sort varchar(50)='id asc' ,--默认 排序表达式
	@where varchar(100) --where条件 注意必须带where关键字
*/
            using (SqlConnection conn =
             new SqlConnection(DbSqlHelper.connstr))
            {
                conn.Open();//打开链接
                SqlCommand cmd = conn.CreateCommand();//创建命令
                //创建输入参数
                SqlParameter p_start = new SqlParameter("@start", start);
                SqlParameter p_end = new SqlParameter("@end", end);
                SqlParameter p_tablename = new SqlParameter("@tablename", "Publishers");
                //如果直接参数构造器中调用赋值，则无需这两句
                SqlParameter p_fields =
                    new SqlParameter("@fields", "id,name");
                SqlParameter p_where =
                    new SqlParameter("@where", "");//where条件
                SqlParameter p_sort =
                  new SqlParameter("@sort", "id asc ");//默认排序条件

                //按照存储过程定义参数的顺序，添加参数
                cmd.Parameters.Add(p_tablename);
                cmd.Parameters.Add(p_fields);
                cmd.Parameters.Add(p_start);
                cmd.Parameters.Add(p_end);
                cmd.Parameters.Add(p_sort);
                cmd.Parameters.Add(p_where);
                
               
                //设置cmd调用格式为存储过程
                cmd.CommandType = CommandType.StoredProcedure;
                //设置调用存储过程名字
                cmd.CommandText = "proc_page";

                //cmd.ExecuteNonQuery(); 如果不返回结果集，就这样调用
                //p_sort.Value 检索某个输出参数值
                //这个存储过程是返回结果集的
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);//填充结果集
                List<Publisher> list = new List<Publisher>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Publisher p = new Publisher();
                    p.Id = Convert.ToInt32(row["id"]);
                    p.Name = Convert.ToString(row["name"]);
                    list.Add(p);
                }
                return list;

            }

        }
        /// <summary>
        /// 得到出版社总记录数
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount()
        {
          
            string sql ="select count(id) from publishers";
            return Convert.ToInt32(DbSqlHelper.ExecuteScalar(sql));
        }

        /// <summary>
        /// 根据出版社编号获得出版社对象
        /// </summary>
        /// <param name="id">出版社编号</param>
        /// <returns>出版社对象，如果不存在，则null</returns>
        public Publisher GetSingle(int id) {
            //select * from Publishers where  id=35
            string sql = string.Format("select * from Publishers where  id={0}", id);
            DataSet ds = DbSqlHelper.Query(sql);
            Publisher p = null;
            if (ds.Tables[0].Rows.Count > 0) { 
            //存在
                p = new Publisher();
                p.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
                p.Name = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);
            }
            return p;
        
        }
        /// <summary>
        /// 根据出版社编号删除出版社
        /// </summary>
        /// <param name="id">出版社编号</param>
        public void Delete(int id) {
            //delete from Publishers where id=35
            string sql = string.Format("delete from Publishers where id={0}", id);
            DbSqlHelper.ExecuteSql(sql);
        }
    }
}