using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models
{
    /// <summary>
    /// 委托演示，委托是一种特殊的类型，指向一个方法
    /// 这个委托类型，指向一个方法，带两个int参数，返回int
    /// </summary>
    
    public delegate int MyDelegate1(int a,int b);

    public delegate void MyDelegate2(int a, int b);
    //泛型委托
    public delegate bool MyFunc<T>(T t);

    public class MyMath{
        public int Add(int a,int b){
            return a+b;
        }
        public int Minus(int a, int b)
        {
            return a - b;
        }
        public void Add1(int a, int b)
        {
            HttpContext.Current.Response.Write( a + b);
        }
        public void Minus1(int a, int b)
        {
            HttpContext.Current.Response.Write(a - b);
        }


    }
    
}
