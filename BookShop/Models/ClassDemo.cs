using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models
{
    /// <summary>
    /// 第五章：泛型类和泛型方法，仅仅是作为知识点演示，本类没有意义
    /// 这就是泛型类,泛型T，代表任意的类型，在具体调用时给类型
    /// 在类中可以使用泛型T类型
    /// </summary>
    public class ClassDemo<T>
    {
        public string Show(T t) {
            return t.ToString();
        }
        /// <summary>
        /// 这是一个泛型方法，泛型K，只有在调用时确定‘
        /// where K :new () 这是可选的，对泛型K进行约束，这里表示
        /// 这个泛型K必须能够被实例化
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="k"></param>
        /// <returns></returns>
        public string Fun<K>() where K :new () {
            K k = new K();
            return k.ToString();
        }
    }
}