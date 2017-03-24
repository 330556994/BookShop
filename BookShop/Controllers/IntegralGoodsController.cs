using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;
using BookShop.Models.Services;
using BookShop.Models.Entities;

namespace BookShop.Controllers
{
    public class IntegralGoodsController : Controller
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();

        private UserService userService = new UserService();
    

        //
        // GET: /IntegralGoods/


        /// <summary>
        /// 积分兑换首页
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            userService.CheckUser();

            //获得可用积分
            //var name = Session["name"].ToString();
            //var score= userService.GetUserById(userService.GetUserIdByLoginId(name)).scoreCurrent;

            int userid = userService.GetUserId(Session["name"].ToString());
            var score = db.Users.Where(p => p.Id == userid).FirstOrDefault().scoreCurrent;
            ViewBag.Score = score;

                          
            //积分区段
            List<IntegralSection> integralsection = db.IntegralSection.ToList();         
            ViewBag.IntegralSections = integralsection;

            return View();
        }




        /// <summary>
        /// 点击区段后显示的商品页面，供ajax调用
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public ActionResult IntegralSection_ajax(int sectionId)
        {          
            //获得对应区段下的积分商品
            List<IntegralGoods> integralgoods = null;
            if (sectionId <= 0)
            {
                integralgoods = db.IntegralGoods.Include(i => i.IntegralSection).ToList();
            }
            else
            {
                integralgoods = db.IntegralGoods.Include(i => i.IntegralSection).Where(i => i.IntegralSectionId == sectionId).ToList();
            }
          
            return  PartialView(integralgoods);
        }



      
    }
}