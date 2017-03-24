using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;//导入绘图命名空间
namespace BookShop.Tools
{
    /// <summary>
    /// 文件助手帮助类，用在文件上传中
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 这个变量表示允许上传的文件类型，多种类型用逗号间隔
        /// </summary>
        public static string FileMode = "jpg,gif,png,bmp";
        /// <summary>
        /// 创建一个不重复的文件名 规则是yyyymmddhhmmssmsmsms
        /// </summary>
        /// <returns></returns>
        public static string CreateFileName() {            
            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
               System.DateTime .Now.Year
               , System.DateTime.Now.Month
               , System.DateTime.Now.Day
               , System.DateTime.Now.Hour
               , System.DateTime.Now.Minute
               , System.DateTime.Now.Second
               , System.DateTime.Now.Millisecond);
        }
        /// <summary>
        /// 判断filename的扩展名是否是允许的范围
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckFileName(string fileName) { 
            var part=fileName.Split('.');
            string extname = part[part.Length - 1];//得到文件扩展名
            return FileMode.Contains(extname.ToLower()); 
        }
        /// <summary>
        /// 得到文件扩展名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetExtFileName(string fileName){
            var part = fileName.Split('.');
            string extname = part[part.Length - 1];//得到文件扩展名
            return extname;
        
        }

     
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="Length">生成长度</param>
   
        /// <returns></returns>
        public static string CreateRandomCode(int Length)
        {
          
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }


        public static  byte[] CreateValidateGraphic(string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 15);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 30);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.LightCyan);
            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple, Color.SkyBlue };
            //定义字体 
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "Comic Sans MS" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 10; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                int colorindex = rand.Next(7);
                g.DrawPie(new Pen(c[colorindex], 0), x, y, 6, 6, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(6);
                Font _font = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                int ii = 4;
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }
                g.DrawString(checkCode.Substring(i, 1), _font, b, 3 + (i * 12), ii);
            }

            //画一个边框
            g.DrawRectangle(new Pen(Color.Red, 0), 100, 0, image.Width - 1, image.Height - 1);
            //输出到浏览器
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            return ms.ToArray();
        }

            
    }
}