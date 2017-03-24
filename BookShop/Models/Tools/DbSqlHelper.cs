using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//数据集  
using System.Data;
//数据提供者
using System.Data.SqlClient;
//Accp代表公司名称
namespace Accp.Tools
{
    /// <summary>
    /// 数据库操作工具类
    /// </summary>
    public class DbSqlHelper
    {
        public static string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;//数据库连接字符串
        /// <summary>
        /// 传入select语句，返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet Query(string sql) {
            using (SqlConnection conn = new SqlConnection(connstr)) {
                conn.Open();//打开
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);//填充数据集
                return ds;
            }
        }
        /// <summary>
        /// 执行sql命令，如insert,delete,update
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sql) {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();//打开
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();              
            }
        }
        //执行sql语句，返回首行首列
        public static object ExecuteScalar(string sql) {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();//打开
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                return cmd.ExecuteScalar();
            }
        }

        public static int ExecuteProc(object param, out Dictionary<string,object> outparams) { 
            //匿名对象 约定 proc_name 固定写法 表示存储过程的名字 i_表示的输入参数  i_p1表示 存储过程输参数，名字是 p1
            //o_表示的是输出参数， o_param表示 输出参数名字是 param
            var obj = new { proc_name = "proc_test", i_p1 = 1, i_p2 = "abc" };
            outparams = null;
            return 1;
        }
    }
}