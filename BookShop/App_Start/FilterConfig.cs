using System.Web;
using System.Web.Mvc;

namespace BookShop
{
    public class FilterConfig
    {

        /// <summary>
        /// 这里是配置全局过滤器
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
         //   filters.Add(new HandleErrorAttribute());
            //filters.Add(new BookShop.Models.MyValidate());
           // filters.Add(new BookShop.Models.MySqlValidate());
        }
    }
}