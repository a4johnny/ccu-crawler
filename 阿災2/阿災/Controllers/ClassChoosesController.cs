using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text; // encoding
using System.Web;
using System.Web.Mvc;
using 阿災.Models;

namespace 阿災.Controllers
{
    public class ClassChoosesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClassChooses
        public ActionResult Index()
        {
            return View(db.ClassChooses.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int a = 1)
        {
            HtmlWeb htmlweb = new HtmlWeb();
            htmlweb.OverrideEncoding = Encoding.GetEncoding("utf-8"); //校正編碼
            var doc = htmlweb.Load("https://kiki.ccu.edu.tw/~ccmisp06/Course/"); //抓網頁
            // items 包含 xml 底下所有node
            //HtmlNodeCollection items = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]");
            //再往下分成一個個小群node
            int num = 0;
            for (var i = 1; i <= 8; i++)
            {   // 分四層  網頁架構問題  我不曉得他為甚麼他要把code寫得跟金字塔一樣
                HtmlNodeCollection items1 = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[" + i.ToString() + "]/font/a");
                HtmlNodeCollection items2 = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[" + i.ToString() + "]/font/font/a");
                HtmlNodeCollection items3 = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[" + i.ToString() + "]/font/font/font/a");
                HtmlNodeCollection items4 = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[" + i.ToString() + "]/font/font/font/font/a");

                string href = "";
                string Cname = "";
                // 重複四遍  遇到無網址的地方就會xml跳到下一層 (item1 > item2)
                if (items1 != null)
                {
                    for (var j = 0; j < items1.Count; j++)
                    {   // 抓課程名稱 和 網址
                        Cname = items1[j].InnerText;
                        href = items1[j].GetAttributeValue("href", null);
                        href = "https://kiki.ccu.edu.tw/~ccmisp06/Course/" + href;
                        //寫入 db
                        ClassChoose one = new ClassChoose();
                        one.Id = Guid.NewGuid();
                        one.classname = Cname;
                        one.classurl = href;
                        one.num = num;
                        num++;
                        db.ClassChooses.Add(one);
                        db.SaveChanges();
                    }
                }

                if (items2 != null)
                {
                    for (var j = 0; j < items2.Count; j++)
                    {
                        Cname = items2[j].InnerText;
                        href = items2[j].GetAttributeValue("href", null);
                        href = "https://kiki.ccu.edu.tw/~ccmisp06/Course/" + href;

                        ClassChoose one = new ClassChoose();
                        one.Id = Guid.NewGuid();
                        one.classname = Cname;
                        one.classurl = href;
                        one.num = num;
                        num++;
                        db.ClassChooses.Add(one);
                        db.SaveChanges();
                    }
                }

                if (items3 != null)
                {
                    for (var j = 0; j < items3.Count; j++)
                    {
                        Cname = items3[j].InnerText;
                        href = items3[j].GetAttributeValue("href", null);
                        href = "https://kiki.ccu.edu.tw/~ccmisp06/Course/" + href;

                        ClassChoose one = new ClassChoose();
                        one.Id = Guid.NewGuid();
                        one.classname = Cname;
                        one.classurl = href;
                        one.num = num;
                        num++;
                        db.ClassChooses.Add(one);
                        db.SaveChanges();
                    }
                }

                if (items4 != null)
                {
                    for (var j = 0; j < items4.Count; j++)
                    {
                        Cname = items4[j].InnerText;
                        href = items4[j].GetAttributeValue("href", null);
                        href = "https://kiki.ccu.edu.tw/~ccmisp06/Course/" + href;

                        ClassChoose one = new ClassChoose();
                        one.Id = Guid.NewGuid();
                        one.classname = Cname;
                        one.classurl = href;
                        one.num = num;
                        num++;
                        db.ClassChooses.Add(one);
                        db.SaveChanges();
                    }
                }
            }
            // 爬最後一排的課程 (格式不同)
            for (var k = 1; k < 30; k++)
            {
                HtmlNodeCollection itemsx = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[9]/font/a[" + k.ToString() + "]");
                if (itemsx == null)
                {
                    k = 31;
                }
                else
                {
                    ClassChoose one = new ClassChoose();
                    one.Id = Guid.NewGuid();
                    one.classname = itemsx[0].InnerText;
                    string href = itemsx[0].GetAttributeValue("href", null); // 抓 url
                    href = href.Remove(0, 2); //起始位置,長度 // 刪掉最前面的兩個點
                    one.classurl = "https://kiki.ccu.edu.tw/~ccmisp06" + href;
                    one.num = num;
                    num++;
                    db.ClassChooses.Add(one);
                    db.SaveChanges();
                }
            }

            //var node = items[0].SelectSingleNode("tr[2]/td[2]/font/a");
            //var classname = node.InnerText;
            //var url = node.Attributes["href"].Value;
            return View(db.ClassChooses.ToList());
        }

        // GET: ClassChooses/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassChoose classChoose = db.ClassChooses.Find(id);
            if (classChoose == null)
            {
                return HttpNotFound();
            }
            return View(classChoose);
        }

        // GET: ClassChooses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClassChooses/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,classname,classurl")] ClassChoose classChoose)
        {
            if (ModelState.IsValid)
            {
                classChoose.Id = Guid.NewGuid();
                db.ClassChooses.Add(classChoose);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(classChoose);
        }

        // GET: ClassChooses/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassChoose classChoose = db.ClassChooses.Find(id);
            if (classChoose == null)
            {
                return HttpNotFound();
            }
            return View(classChoose);
        }

        // POST: ClassChooses/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,classname,classurl")] ClassChoose classChoose)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classChoose).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classChoose);
        }

        // GET: ClassChooses/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassChoose classChoose = db.ClassChooses.Find(id);
            if (classChoose == null)
            {
                return HttpNotFound();
            }
            return View(classChoose);
        }

        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAll(Guid? id)
        {
            ClassChoose one = new ClassChoose();
            while (db.ClassChooses.Where(p => p.Id != null) != null)
            {
                db.ClassChooses.RemoveRange(db.ClassChooses.Where(x => x.Id != null));
            }
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        // POST: ClassChooses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id, string action)
        {
            if (action == "Delete")
            {
                ClassChoose one = db.ClassChooses.Find(id);
                db.ClassChooses.Remove(one);
                db.SaveChanges();
            }
            else if (action == "DeleteAll")
            {
                ClassChoose one = new ClassChoose();
                int count = db.ClassChooses.ToList().Count;
                while (count != 0)
                {
                    ClassChoose one1 = db.ClassChooses.Where(x => x.Id != null).FirstOrDefault();
                    db.ClassChooses.Remove(one1);
                    count--;
                    db.SaveChanges();
                }
            }
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
