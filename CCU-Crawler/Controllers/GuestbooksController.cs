using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCU_Crawler.Models;

namespace CCU_Crawler.Controllers
{
    public class GuestbooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Guestbooks
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Guestbooks/Details/5

        public ActionResult List(int? id) // 列出選中的課程評論
        {   
            if (id == null)
            {
                return RedirectToAction("Index");
            }  
            Course course = db.Courses.Find(id);

            TempData["id"] = id; 
            ViewBag.name = course.Name;
            ViewBag.teacher = course.Teacher;
            ViewBag.remark = course.Remark;

            return View(db.Guestbooks.Where(p => p.CourceId == id).ToList()); //只顯示給該課程的評論
        }

        public ActionResult Conplain(int CourseId) //給評論的地方
        {
            TempData["id2"] = CourseId;  // 把傳過來的課程ID繼承下來
            return View();
        }

        [HttpPost] // 按下按鈕觸發後會到這裡
        public ActionResult Conplain(int classid, int score, string content) // 傳入課程ID 給定的分數 評論內容
        {   
            Guestbook guestbook = new Guestbook();
            //guestbook.CourceId = Convert.ToInt32(idd.ToString());  //idd被我殺了  保留寫法
            guestbook.CourceId = classid;
            guestbook.Score = score;
            guestbook.Content = content;
            guestbook.DateTime = DateTime.Now;
            db.Guestbooks.Add(guestbook);
            db.SaveChanges();

            return RedirectToAction("List", new { id = classid});
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guestbook guestbook = db.Guestbooks.Find(id);
            if (guestbook == null)
            {
                return HttpNotFound();
            }
            return View(guestbook);
        }

        // GET: Guestbooks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Guestbooks/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Score,Content")] Guestbook guestbook)
        {
            if (ModelState.IsValid)
            {
                db.Guestbooks.Add(guestbook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(guestbook);
        }

        // GET: Guestbooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guestbook guestbook = db.Guestbooks.Find(id);
            if (guestbook == null)
            {
                return HttpNotFound();
            }
            return View(guestbook);
        }

        // POST: Guestbooks/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Score,Content")] Guestbook guestbook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guestbook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guestbook);
        }

        // GET: Guestbooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guestbook guestbook = db.Guestbooks.Find(id);
            if (guestbook == null)
            {
                return HttpNotFound();
            }
            return View(guestbook);
        }

        // POST: Guestbooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Guestbook guestbook = db.Guestbooks.Find(id);
            db.Guestbooks.Remove(guestbook);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
