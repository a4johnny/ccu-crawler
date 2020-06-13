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

            course.Popularity = db.Guestbooks.Where(p => p.CourceId == id).ToList().Count(); //初始化資料庫用 可刪除
            db.SaveChanges();                                                                //同上

            return View(db.Guestbooks.Where(p => p.CourceId == id).ToList()); //只顯示給該課程的評論
        }

        [HttpPost]
        public ActionResult List(int CourseId, int CommentId, string action) //給評論的地方
        {
            Comment comment = new Comment();
            comment.CourseId = CourseId;
            comment.CommentId = CommentId;
            if (action == "讚")
            {
                comment.GoodOrBad = 1;
            }
            else if (action == "評論Bad")
            {
                comment.GoodOrBad = 0;
            }

            if (User.Identity.Name != null && User.Identity.Name != "")
                comment.User = User.Identity.Name;
            else
                comment.User = "stranger";

            Guestbook guestbook = new Guestbook();
            guestbook = db.Guestbooks.Where(x => x.Id == CommentId).FirstOrDefault();
            if (comment.GoodOrBad == 1)
                guestbook.good++;
            else if (comment.GoodOrBad == 0)
                guestbook.bad++;

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("List", new { id = CourseId });
        }

        public ActionResult Conplain(int CourseId) //給評論的地方
        {
            TempData["id2"] = CourseId;  // 把傳過來的課程ID繼承下來
            return View();
        }

        [HttpPost] // 按下按鈕觸發後會到這裡
        public ActionResult Conplain(int classid, int? score, string content, int? call, int? sign, int? group) // 傳入課程ID 給定的分數 評論內容
        {
            TempData["warning"] = null;
            if (score == null) {
                TempData["warning"] = "評分不可為空值";
                return RedirectToAction("Conplain", new { CourseId = classid });
            }

            Guestbook guestbook = new Guestbook();
            //guestbook.CourceId = Convert.ToInt32(idd.ToString());  //idd被我殺了  保留寫法
            guestbook.CourceId = classid;
            guestbook.Score = (int)score;
            guestbook.Content = content;
            guestbook.Call = call;
            guestbook.Sign = sign;
            guestbook.Group = group;

            if (call == 1)
                guestbook.Infomation = guestbook.Infomation + "時常點名 ";
            else if (call == 2)
                guestbook.Infomation = guestbook.Infomation + "偶爾點名 ";
            else if (call == 3)
                guestbook.Infomation = guestbook.Infomation + "不需點名 ";

            if(sign == 1)
                guestbook.Infomation = guestbook.Infomation + "無條件加簽 ";
            else if (sign == 2)
                guestbook.Infomation = guestbook.Infomation + "有條件加簽 ";
            else if (sign == 3)
                guestbook.Infomation = guestbook.Infomation + "不可加簽 ";

            if (group == 1)
                guestbook.Infomation = guestbook.Infomation + "需要分組";

            guestbook.good = 0;
            guestbook.bad = 0;
            guestbook.DateTime = DateTime.Now;
            db.Guestbooks.Add(guestbook);

            Course course = db.Courses.Find(classid);
            course.Popularity++;

            db.SaveChanges();

            return RedirectToAction("List", new { id = classid });
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
