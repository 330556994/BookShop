using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BookShop.Areas.Admin.Models;

namespace BookShop.Areas.Admin.Controllers
{
    public class CategoryManageController : Controller
    {
        //
        // GET: /Admin/CategoryManage/
        /// <summary>
        /// 演示EF框架使用
        /// </summary>
        /// <returns></returns>
        /// 
        BookShopPlusEntities db = new BookShopPlusEntities();
        /// <summary>
        ///  当对象消亡时自动会调用，这里释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();//关闭db，并释放
            base.Dispose(disposing);
        }
        public ActionResult Index()
        {
            //ef框架主操作类
            //不要状态管理
            db.Configuration.AutoDetectChangesEnabled = false;
            //可以使用扩展方法，where
            var list = db.Categories.Where(p => p.Id > 10);
            return View(list);
        }
        /// <summary>
        /// 实现删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id) {
           
            //删除之前先要查出来
          //  var c = db.Categories.SingleOrDefault(p => p.Id == id); // db.Categories.Find(id); //根据主键来查
            string html;
            try
            {
                //在集合里删除,这个过程并没有真正删除，而是在集合里，
                //把这个对象的状态设置成删除状态
               // db.Categories.Remove(c);
                //根据实体状态，来生成不同的SQL语句，并完成数据库同步
                ///////////////////////////////
                //删除的第二种做法，这样性能更好
                ////////////////////////////
                //先创建替身对象
                Categories c = new Categories{ Id=id};
                db.Categories.Attach(c);//给定实体附加到上下文中
                db.Categories.Remove(c);//删除
                db.SaveChanges();//提交到数据库，完成修改

                //跳转到列表动作
                return RedirectToAction("index");
            }
            catch (Exception ex) {
                html = "<script>alert('删除失败');location.href='/admin/categorymanage/index';</script>";
                return Content(html);
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create() {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Categories category) {

         
            string html;
            if (ModelState.IsValid == true)
            {
                try
                {
                    //在集合里添加,这个过程并没有真正添加，而是在集合里，
                    //把这个对象的状态设置成新增状态
                    db.Categories.Add(category);
                    //根据实体状态，来生成不同的SQL语句，并完成数据库同步
                    db.SaveChanges();//提交到数据库，完成修改
                    //跳转到列表动作
                    return RedirectToAction("index");
                }
                catch (Exception ex)
                {
                    html = "<script>alert('新增失败');history.go(-1);</script>";
                    return Content(html);
                }
            }
            else {
                return View();
            }
        }

        public ActionResult Edit(int id) {
          
            var c = db.Categories.SingleOrDefault(p => p.Id == id);
            if (c == null)
            {
                return Content("<script>alert('没有该记录');history.go(-1);</script>");

            }
            else {
                return View(c);
            }
        }

        [HttpPost]
        public ActionResult Edit(Categories category)
        {

      
            string html;
            if (ModelState.IsValid == true)
            {
                try
                {
                   //这句意义是指，将这个对象的状态修改成更新状态
                    db.Entry(category).State = System.Data.EntityState.Modified;
                    //根据实体状态，来生成不同的SQL语句，并完成数据库同步
                    db.SaveChanges();//提交到数据库，完成修改
                    //跳转到列表动作
                    return RedirectToAction("index");
                }
                catch (Exception ex)
                {
                    html = "<script>alert('更新失败');history.go(-1);</script>";
                    return Content(html);
                }
            }
            else
            {
                return View();
            }
        }

    }
}
