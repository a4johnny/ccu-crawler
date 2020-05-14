using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CCU_Crawler.Models;
using HtmlAgilityPack;

namespace CCU_Crawler.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Departments
        public ActionResult Index()
        {
            var web = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8
            };

            var doc = web.Load("https://kiki.ccu.edu.tw/~ccmisp06/Course/");
            var departmentNames = db.Departments.Select(department => department.Name).ToList();
            var count = departmentNames.Count;
            var suffix = "/a";

            for (var s = 1; s < 5; s++)
            {
                suffix = "/font" + suffix;

                for (var i = 1; i <= 8; i++)
                {
                    HtmlNodeCollection items = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[2]/td[" + i.ToString() + "]" + suffix);

                    if (items != null)
                    {
                        for (var j = 0; j < items.Count; j++)
                        {
                            var name = items[j].InnerText;

                            if (!CheckNameExist(departmentNames, name))
                            {
                                var url = items[j].GetAttributeValue("href", null);
                                url = "https://kiki.ccu.edu.tw/~ccmisp06/Course/" + url;

                                Department department = new Department
                                {
                                    Id = count,
                                    Name = name,
                                    Url = url
                                };
                                count++;
                                db.Departments.Add(department);
                                db.SaveChanges();
                            }
                        }
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
                    if (!CheckNameExist(departmentNames, itemsx[0].InnerText))
                    {
                        Department department = new Department
                        {
                            Id = count,
                            Name = itemsx[0].InnerText
                        };
                        string href = itemsx[0].GetAttributeValue("href", null); // 抓 url
                        href = href.Remove(0, 2); //起始位置,長度 // 刪掉最前面的兩個點
                        department.Url = "https://kiki.ccu.edu.tw/~ccmisp06" + href;
                        count++;
                        db.Departments.Add(department);
                        db.SaveChanges();
                    }
                }
            }

            return View(db.Departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CollegeId,Name,Url")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CollegeId,Name,Url")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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

        private bool CheckNameExist(List<string> departmentNames, string departmentName)
        {
            foreach (var name in departmentNames)
            {
                if (departmentName == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
