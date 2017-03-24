using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookShop.Areas.Admin.Models;

namespace BookShop.Areas.Admin.Controllers
{
    [Authorize] //在控制器里添加这个特性，表示该控制器里的所有action都需要身份验证
    public class PublisherManageController : Controller
    {
        private BookShopPlusEntities db = new BookShopPlusEntities();

        //
        // GET: /Admin/PublisherManage/
        
        public ActionResult Index()
        {
            return View(db.Publishers.ToList());
        }

        //
        // GET: /Admin/PublisherManage/Details/5

        public ActionResult Details(int id = 0)
        {
            Publishers publishers = db.Publishers.Find(id);
            if (publishers == null)
            {
                return HttpNotFound();
            }
            return View(publishers);
        }

        //
        // GET: /Admin/PublisherManage/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/PublisherManage/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Publishers publishers)
        {
            if (ModelState.IsValid)
            {
                db.Publishers.Add(publishers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(publishers);
        }

        //
        // GET: /Admin/PublisherManage/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Publishers publishers = db.Publishers.Find(id);
            if (publishers == null)
            {
                return HttpNotFound();
            }
            return View(publishers);
        }

        //
        // POST: /Admin/PublisherManage/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Publishers publishers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publishers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(publishers);
        }

        //
        // GET: /Admin/PublisherManage/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Publishers publishers = db.Publishers.Find(id);
            if (publishers == null)
            {
                return HttpNotFound();
            }
            return View(publishers);
        }

        //
        // POST: /Admin/PublisherManage/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Publishers publishers = db.Publishers.Find(id);
            db.Publishers.Remove(publishers);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}