using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BookShop.Tools;
namespace BookShop.Controllers
{
    public class ToolsController : Controller
    {
        //
        // GET: /Tools/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 创建一个验证码图片
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateImg() {
            //生成一个随机字符串
            string code = FileHelper.CreateRandomCode(5);
            //将这个字符串保存在临时字典里
            //tempdata实现跨action传递数据,键值对

            TempData["code"] = code;
            //生成该图片字节流
            var imgs = FileHelper.CreateValidateGraphic(code);
            //将imgs图片字节流，以jpeg的格式，写到客户端去
            return File(imgs, "images/Jpeg");
        }

    }
}
