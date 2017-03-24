using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BookShop.Models.Entities;

namespace BookShop.Models.Services
{
    /// <summary>
    /// 购物车的操作类
    /// </summary>
    public class ItemService
    {
        
        /// <summary>
        /// 只读属性，从session中 car 获得购物车集合 Items
        /// </summary>
        public List<Item> Items { 
            get {
                return HttpContext.Current.Session["car"] as List<Item>;
        } }
        /// <summary>
        /// 在购物车里 查询编号为id的购物项
        /// </summary>
        /// <param name="id"></param>
        /// <returns>存在，返回索引，不存在返回-1</returns>
        private int Search(int id) {
            for (int i = 0; i < Items.Count; i++) {
                if (Items[i].Id == id) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 添加购物项到购物车里
        /// 如果购物车中没有购物项则添加，如果存在，则购买数量累加
        /// </summary>
        /// <param name="item"></param>
        public void Add(Item item)
        {
            int ret = Search(item.Id);//先查
            if (ret == -1)
            {
                Items.Add(item);
            }
            else {
                Items[ret].Qty++;
            }
            //保存集合到session中
            HttpContext.Current.Session["car"] = Items;
            SaveCarToCookie();
        
        }
        /// <summary>
        /// 在购物车里修改编号为id的购物项，只修改数量qty
        /// </summary>
        /// <param name="id">待修改的购物项编号</param>
        /// <param name="qty">要修改的购买数量</param>
        public void Update(int id, int qty) {
            int ret = Search(id);//先查
            if (ret != -1) { 
            //如果存在，则修改对应的数量
                Items[ret].Qty = qty;
                //保存集合到session中
                HttpContext.Current.Session["car"] = Items;
                SaveCarToCookie();
            }
          

        }
        /// <summary>
        /// 在购物车里删除编号为id的购物项
        /// </summary>
        /// <param name="id">要删除的数量</param>
        public void Delete(int id) {
            int ret = Search(id);//先查
            if (ret != -1)
            {
                //如果存在，则删除指定索引位置的对象
                Items.RemoveAt(ret);
                //保存集合到session中
                HttpContext.Current.Session["car"] = Items;
                SaveCarToCookie();
            }
        }
        /// <summary>
        /// 清除购物车
        /// </summary>
        public void RemoveAll() {
            Items.Clear();//清除集合内容
            //保存集合到session中
            HttpContext.Current.Session["car"] = Items;
            SaveCarToCookie();
        }
        /// <summary>
        /// 这个方法是将当前购物车的数据保存在cookie里
        /// cookie应当使用多值
        /// key存放id，value存放数量
        /// 本方法应该在
        /// </summary>
        public void SaveCarToCookie() {
            HttpCookie cookie = new HttpCookie("_accp_car");//创建cookie
            foreach (Item item in Items) { 
            //循环购物车集合，并在cookie里保存id,和qty
                cookie.Values[item.Id.ToString()] = item.Qty.ToString();
            }
            cookie.Expires = System.DateTime.Now.AddMonths(3);
            //写入客户端
            HttpContext.Current.Response.Cookies.Add(cookie);
            
        }
        /// <summary>
        /// 从cookie里取出购物车信息，并重建session
        /// 本方法应该在什么时候调用？session_start里调用
        /// </summary>
        public void LoadCarFromCookie() { 
        // 创建集合对象，先得到名字为_accp_car的cookie，判断是否存在
            //如果得到了，则循环这个cookie.Values.AllKeys,所有键
            //其实键就是id，值就是数量，然后，调用图书服务类，得到
            //得到该编号的图书对象，并创建购物项对象，添加到集合里
            //创建session变量，并保存这个集合。
            List<Item> list = new List<Item>();
            BookService bookservice = new BookService();
            HttpCookie cookie = HttpContext.Current.Request.Cookies["_accp_car"];
            if (cookie != null) {
                foreach (string key in cookie.Values.AllKeys) {
                    //得到这本书
                    if (string.IsNullOrEmpty(key) == false)
                    {
                        Book book = bookservice.GetSingle(Convert.ToInt32(key));
                        Item item = new Item();
                        item.Id = book.Id;
                        item.Title = book.Title;
                        item.UnitPrice = book.UnitPrice;
                        item.MarketPrice = book.MarketPrice;
                        item.Qty = Convert.ToInt32(cookie.Values[key]);
                        list.Add(item);
                    }
                }
            }
            HttpContext.Current.Session["car"] = list;

        }
    }
}