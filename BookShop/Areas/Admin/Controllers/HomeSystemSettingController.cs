using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;
using System.Data;


namespace BookShop.Areas.Admin.Controllers
{
    /// <summary>
    /// 作者:李天页
    /// 功能描述:提供系统设置所需要的分布视图
    /// </summary>
    public class HomeSystemSettingController : Controller
    {
        //
        // GET: /Admin/HomeSystemSetting/

        private BookShopPlusEntities db = new BookShopPlusEntities();


        /// <summary>
        /// 作者:李天页
        /// 功能描述:星级评论分布视图，供ajax调用
        /// </summary>
        /// <param name="several">对应为几星级，如值为1则对应为1星级</param>
        /// <returns></returns>
        public ActionResult starComment(int several)
        {
            Config config = null;

            ViewBag.Several = several;

            int defaultValue = 0;
            switch (several) 
            {
                case 1:
                    config = db.Config.Where(p => p.KeyField.Equals("oneStar")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
                case 2:
                    config = db.Config.Where(p => p.KeyField.Equals("twoStar")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
                case 3:
                    config = db.Config.Where(p => p.KeyField.Equals("threeStar")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
            }
            ViewBag.DefaultValue = defaultValue;

            return PartialView ();
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:星级评论修改数值，供ajax调用
        /// </summary>
        /// <param name="num">要修改的数值</param>
        /// <param name="several">几星</param>
        /// <returns></returns>
        public ActionResult editStarComment(int num, int several)
        {
            string json = "";
            Config config = null;
            if(num<0){
                json = "{\"success\":2}";
                return Content(json);
            }
            try
            {
                if (several == 1)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("oneStar")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (several == 2)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("twoStar")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (several == 3)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("threeStar")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json="{\"success\":1}";
            }           
            return Content(json);
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:签到分布视图，供ajax调用
        /// </summary>
        /// <param name="flag">0:单次签到,1:连续签到</param>
        /// <returns></returns>
        public ActionResult SignView(int flag)
        {
            ViewBag.Flag = flag;
            Config config = null;
       
            int defaultValue = 0;
            switch (flag)
            {
                case 0:
                    config = db.Config.Where(p => p.KeyField.Equals("singleSign")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
                case 1:
                    config = db.Config.Where(p => p.KeyField.Equals("continueSign")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;              
            }
            ViewBag.DefaultValue = defaultValue;

            return PartialView();
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:签到修改数值，供ajax调用
        /// </summary>
        /// <param name="flag">状态0:单次签到,1:连续签到</param>
        /// <param name="num">签到积分数值</param>
        /// <returns></returns>
        public ActionResult EditSign(int flag,int num)
        {
            string json = "";
            Config config = null;
            if (num < 0)
            {
                json = "{\"success\":2}";
                return Content(json);
            }
            try
            {
                if (flag == 0)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("singleSign")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (flag == 1)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("continueSign")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }             
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":1}";
            }
            return Content(json);
        }




        /// <summary>
        /// 作者:李天页
        /// 功能描述:确认分布视图，供ajax调用
        /// </summary>
        /// <param name="flag">flag代表是确认收货还是确认订单,0:确认收货,1:确认订单</param>
        /// <returns></returns>
        public ActionResult ConfirmView(int flag)
        {
            ViewBag.Flag = flag;
            Config config = null;

            int defaultValue = 0;
            switch (flag)
            {
                case 0:
                    config = db.Config.Where(p => p.KeyField.Equals("autoReceipt")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
                case 1:
                    config = db.Config.Where(p => p.KeyField.Equals("autoConfirmOrder")).ToList()[0];
                    defaultValue = Convert.ToInt32(config.ValueField);
                    break;
            }
            ViewBag.DefaultValue = defaultValue;

            return PartialView();
        }




        /// <summary>
        /// 作者:李天页
        /// 功能描述:签到修改数值，供ajax调用
        /// </summary>
        /// <param name="flag">flag代表是确认收货还是确认订单,0:确认收货,1:确认订单</param>
        /// <param name="num">数值</param>
        /// <returns></returns>
        public ActionResult EditAutoConfirmTime(int flag, int num)
        {
            string json = "";
            Config config = null;
            if (num < 0)
            {
                json = "{\"success\":2}";
                return Content(json);
            }
            try
            {
                if (flag == 0)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("autoReceipt")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (flag == 1)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("autoConfirmOrder")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":1}";
            }
            return Content(json);
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:分页尺寸分布视图，供ajax调用
        /// </summary>
        /// <returns></returns>
        public ActionResult pageSizeSetting()
        {

            Config config = db.Config.Where(p => p.KeyField.Equals("pagesize")).ToList()[0];

            int defaultValue = Convert.ToInt32(config.ValueField);
          
            ViewBag.DefaultValue = defaultValue;

            return PartialView();
        }



        /// <summary>
        /// 作者:李天页
        /// 功能描述:签到修改数值，供ajax调用
        /// </summary>
        /// <param name="num">数值</param>
        /// <returns></returns>
        public ActionResult EditPageSize(int num)
        {
            string json = "";
            Config config = null;
            if (num <= 0)
            {
                json = "{\"success\":2}";
                return Content(json);
            }
            try
            {
                config = db.Config.Where(p => p.KeyField.Equals("pagesize")).ToList()[0];
                config.ValueField = Convert.ToString(num);
                db.Entry(config).State = EntityState.Modified;
                db.SaveChanges();
                
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":1}";
            }
            return Content(json);
        }




        /// <summary>
        /// 作者:李天页
        /// 功能描述:系统是否关闭，分页视图，ajax调用
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemOpenClose()
        {
            Config config = db.Config.Where(p => p.KeyField.Equals("systemOnOff")).ToList()[0];

            int defaultValue = Convert.ToInt32(config.ValueField);

            ViewBag.DefaultValue = defaultValue;

            return PartialView();
        }

        


        /// <summary>
        /// 作者:李天页
        /// 功能描述:系统是否关闭，分页视图，ajax调用
        /// </summary>
        /// <param name="flag">系统是否关闭,0:开启,1:关闭</param>
        /// <returns></returns>
        public ActionResult OpenOrClose(int flag)
        {
            string json = "";
            Config config = null;
            int errorFlag = 3;

            try
            {
                config = db.Config.Where(p => p.KeyField.Equals("systemOnOff")).ToList()[0];
                if (Convert.ToInt32(config.ValueField) != flag)
                {
                    config.ValueField = Convert.ToString(flag);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                    json = string.Format("{0}\"success\":0,\"flag\":{1}{2}", "{", flag, "}");
                }
                else
                {
                    json = string.Format("{0}\"success\":1,\"flag\":{1}{2}", "{", flag, "}");
                }
            }
            catch (Exception ex)
            {
                json = string.Format("{0}\"success\":2,\"flag\":{1}{2}", "{", errorFlag, "}");
            }
            return Content(json);
        }




        /// <summary>
        /// 作者:李天页
        /// 功能描述:钱换积分的分布视图，ajax调用
        /// </summary>
        /// <param name="flag">0:钱直接换积分,1:订单价值增加积分</param>
        /// <returns></returns>
        public ActionResult MoneyToInteView(int flag)
        {
            ViewBag.Flag = flag;

            Config config = null;

            string defaultValue = "";
            switch (flag)
            {
                case 0:
                    config = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralNoBuy")).ToList()[0];
                    defaultValue = Convert.ToString(config.ValueField);
                    break;
                case 1:
                    config = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralBuy")).ToList()[0];
                    defaultValue = Convert.ToString(config.ValueField);
                    break;
            }
            ViewBag.DefaultValue = defaultValue;
             
            return PartialView();
        }




        /// <summary>
        /// 作者:李天页
        /// 功能描述:钱换积分的处理动作，ajax调用
        /// </summary>
        /// <param name="value">可兑换多少积分</param>
        /// <param name="flag">0:钱直接换积分,1:订单价值增加积分</param>
        /// <returns></returns>
        public ActionResult MoneyToInteHandle(int value,int flag)
        {
            string json = "";
            Config config = null;
            if (value < 1)
            {
                json = "{\"success\":2}";
                return Content(json);
            }
            string num = "1:"+value;
            try
            {
                if (flag == 0)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralNoBuy")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (flag == 1)
                {
                    config = db.Config.Where(p => p.KeyField.Equals("moneyToIntegralBuy")).ToList()[0];
                    config.ValueField = Convert.ToString(num);
                    db.Entry(config).State = EntityState.Modified;
                    db.SaveChanges();
                }
                json = "{\"success\":0}";
            }
            catch (Exception ex)
            {
                json = "{\"success\":1}";
            }
            return Content(json);
        }


    }
}
