using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookShop.Areas.Admin.Models;

namespace BookShop.Models.Services
{
    //积分商品业务类
    public class IntegralGoodService
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();

        /// <summary>
        /// 根据商品id求出对应商品
        /// 有则返回商品对象，否则返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IntegralGoods getGoodById(int id)
        {
            var good = db.IntegralGoods.Find(id);
            return good;
        }

    }
}