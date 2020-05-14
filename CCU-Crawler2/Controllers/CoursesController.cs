using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CCU_Crawler.Models;
using HtmlAgilityPack;

namespace CCU_Crawler.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            if (ModelState.IsValid)
            {
                var client = new MyWebClinet();
                var departments = db.Departments.ToList();
                var courseNames = db.Courses.Select(course => course.Name).ToList();
                var count = courseNames.Count;
                var index = 0;

                foreach (var department in departments)
                {
                    var crossFieldCourse = department.Url.Length > 50 ? true : false; // true為跨領域課程

                    // 判斷是不是碩士生課程和通識 備註欄位置不同
                    var different = 0;
                    if (department.Name.IndexOf("碩", 0) > 0 || department.Name.IndexOf("所", 0) > 0 || department.Name.IndexOf("通識中心", 0) > 0)
                    {
                        different = 1;
                    }
                    //HtmlDocument doc = clienta.GetPage("https://kiki.ccu.edu.tw/~ccmisp06/Course/4106.html");
                    //拿 html 檔
                    HtmlDocument doc = client.GetPage(department.Url);
                    ///抓 xpath
                    HtmlNodeCollection list = doc.DocumentNode.SelectNodes("/html/body/center");

                    if (!crossFieldCourse)
                    {
                        int courseIndex = 2;
                        for (int i = 0; i < 300; i++)
                        {   //xpath node 不能加 tbody 不然會crash 原因不明
                            // 通識中心格式不同 多一格向度 向度的資料跟備註放在一起

                            var patterns = new string[14];
                            /*
                             * 0  = Grade
                             * 1  = Number
                             * 2  = Class
                             * 3  = Name
                             * 4  = Teacher
                             * 5  = Duration
                             * 6  = Credit
                             * 7  = Type
                             * 8  = Time
                             * 9  = Location
                             * 10 = Limit
                             * 11 = Url
                             * 12 = Remark
                             * 13 = Dimension
                             */
                            var aPlus = "a"; // 先宣告 不然在其他 fuction 看不到

                            if (department.Name == "通識中心")  //通識中心前面多了一個 向度欄位  所有 XML 向後退一格
                            {
                                patterns[1] = "table/tr[" + courseIndex.ToString() + "]/td[3]/font";
                                patterns[2] = "table/tr[" + courseIndex.ToString() + "]/td[4]/font";
                                patterns[3] = "table/tr[" + courseIndex.ToString() + "]/td[5]/font";
                                patterns[4] = "table/tr[" + courseIndex.ToString() + "]/td[6]/font";
                                patterns[5] = "table/tr[" + courseIndex.ToString() + "]/td[7]/font";
                                patterns[6] = "table/tr[" + courseIndex.ToString() + "]/td[8]/font";
                                patterns[7] = "table/tr[" + courseIndex.ToString() + "]/td[9]/font";
                                patterns[8] = "table/tr[" + courseIndex.ToString() + "]/td[10]/font";
                                patterns[9] = "table/tr[" + courseIndex.ToString() + "]/td[11]/font";
                                patterns[10] = "table/tr[" + courseIndex.ToString() + "]/td[12]/font";
                                patterns[11] = "table/tr[" + courseIndex.ToString() + "]/td[13]/font";
                                patterns[12] = "table/tr[" + courseIndex.ToString() + "]/td[14]/font";
                                patterns[13] = "table/tr[" + courseIndex.ToString() + "]/td[2]";
                            }
                            else
                            {
                                patterns[0] = "table/tr[" + courseIndex.ToString() + "]/td[1]/font";
                                patterns[1] = "table/tr[" + courseIndex.ToString() + "]/td[2]/font";
                                patterns[2] = "table/tr[" + courseIndex.ToString() + "]/td[3]/font";
                                patterns[3] = "table/tr[" + courseIndex.ToString() + "]/td[4]/font";
                                patterns[4] = "table/tr[" + courseIndex.ToString() + "]/td[5]/font";
                                patterns[5] = "table/tr[" + courseIndex.ToString() + "]/td[6]/font";
                                patterns[6] = "table/tr[" + courseIndex.ToString() + "]/td[7]/font";
                                patterns[7] = "table/tr[" + courseIndex.ToString() + "]/td[8]/font";
                                patterns[8] = "table/tr[" + courseIndex.ToString() + "]/td[9]/font";
                                patterns[9] = "table/tr[" + courseIndex.ToString() + "]/td[10]/font";
                                patterns[10] = "table/tr[" + courseIndex.ToString() + "]/td[11]/font";
                                patterns[11] = "table/tr[" + courseIndex.ToString() + "]/td[12]/font";
                                patterns[12] = "table/tr[" + courseIndex.ToString() + "]/td[13]/font";
                            }

                            if (different == 1) // 碩士班比學士班多一格學制 different = 1 表示為碩士班課程
                            {
                                patterns[12] = "table/tr[" + courseIndex.ToString() + "]/td[14]/font";
                            }
                            else
                            {
                                patterns[12] = "table/tr[" + courseIndex.ToString() + "]/td[13]/font";
                            }


                            if (list[0].SelectSingleNode(patterns[3]) == null) // 抓不到東西 跳出迴圈
                            {
                                i = 1000;
                            }
                            else
                            {
                                var courseInfo = new string[14];
                                /*
                                 * 0  = Grade
                                 * 1  = Number
                                 * 2  = Class
                                 * 3  = Name
                                 * 4  = Teacher
                                 * 5  = Duration
                                 * 6  = Credit
                                 * 7  = Type
                                 * 8  = Time
                                 * 9  = Location
                                 * 10 = Limit
                                 * 11 = Url
                                 * 12 = Remark
                                 * 13 = Dimension
                                 */

                                for (var infoIndex = 0; infoIndex < 13; infoIndex++)
                                {
                                    if (patterns[infoIndex] != null)
                                    {
                                        courseInfo[infoIndex] = list[0].SelectSingleNode(patterns[infoIndex]).InnerText;
                                    }
                                    else
                                    {
                                        courseInfo[infoIndex] = "0";
                                    }
                                }

                                if (department.Name == "通識中心")
                                {
                                    aPlus = list[0].SelectSingleNode(patterns[13]).InnerText; //向度 通識專屬
                                }
                                //Console.WriteLine(a1 + a2 + a3 + a4);

                                if (!CheckNameExist(courseNames, courseInfo[3]))
                                {
                                    var course = new Course
                                    {
                                        Id = index,
                                        DepartmentId = department.Id,
                                        Grade = int.Parse(courseInfo[0]),
                                        Number = courseInfo[1],
                                        Class = courseInfo[2],
                                        Name = courseInfo[3],
                                        Teacher = courseInfo[4],
                                        Duration = courseInfo[5],
                                        Credit = int.Parse(courseInfo[6]),
                                        Type = courseInfo[7],
                                        Time = courseInfo[8],
                                        Location = courseInfo[9],
                                        Limit = int.Parse(courseInfo[10]),
                                        Url = courseInfo[11],
                                        Remark = courseInfo[12]
                                    };

                                    if (course.Remark == "")// 備註為空 補上 none
                                    {
                                        course.Remark = "none";
                                    }
                                    course.DepartmentId = department.Id;
                                    index++;
                                    if (department.Name == "通識中心") //通識中心的向度 補在 備註內 不用在多一個 db 欄位去記
                                    {
                                        course.Remark = course.Remark + ",向度:" + aPlus;
                                    }
                                    /*
                                    Debug.WriteLine(course.Id);
                                    Debug.WriteLine(course.DepartmentId);
                                    Debug.WriteLine(course.Grade);
                                    Debug.WriteLine(course.Number);
                                    Debug.WriteLine(course.Class);
                                    Debug.WriteLine(course.Name);
                                    Debug.WriteLine(course.Teacher);
                                    Debug.WriteLine(course.Duration);
                                    Debug.WriteLine(course.Credit);
                                    Debug.WriteLine(course.Type);
                                    Debug.WriteLine(course.Time);
                                    Debug.WriteLine(course.Location);
                                    Debug.WriteLine(course.Limit);
                                    Debug.WriteLine(course.Url);
                                    Debug.WriteLine(course.Remark);
                                    Debug.WriteLine("+++++++++++++++++++++++++++++++++");
                                    */
                                    db.Courses.Add(course);
                                    db.SaveChanges();
                                    courseIndex++;
                                }
                            }
                        }
                    }
                    else if (crossFieldCourse) // 處理 跨領域課程 基本上都是重複的課程 所以這裡只把它有跨的領域 寫到備註裡
                    {   //xml path
                        HtmlNodeCollection list2 = doc.DocumentNode.SelectNodes("/html/body/span/p[2]/center/table");

                        for (var i = 2; i < 30; i++)
                        {
                            string aaa = "tr[" + i.ToString() + "]/td[3]"; // 課程名稱的 xml path 節點
                            if (list2[0].SelectSingleNode(aaa) != null) // 回傳不是 null 就繼續抓
                            {
                                string a6 = list2[0].SelectSingleNode(aaa).InnerText; // aaa 課程名稱的 xml path
                                //Crawler temp = db.Crawlers.Where(p => p.Name.IndexOf(a6,0) != -1).FirstOrDefault(); //indexOf 沒辦法轉成 SQL 語法 所以不能使用
                                //SqlFunctions.CharIndex >> 可以改用這個 但我沒試過
                                var temp = db.Courses.Where(p => p.Name.Contains(a6)).FirstOrDefault(); //找到課程名稱相同的人
                                temp.Remark = temp.Remark + ",跨領域課程:" + department.Name; //寫進備註

                                //Debug.WriteLine(temp.Id);
                                //Debug.WriteLine(temp.Name);
                                //Debug.WriteLine(temp.Teacher);
                                //Debug.WriteLine(temp.Time);
                                //Debug.WriteLine(temp.Location);
                                //Debug.WriteLine(temp.Remark);
                                //Debug.WriteLine("---------------------------------");
                                //db.SaveChanges();
                            }
                            else // 跳出迴圈
                            {
                                i = 31;
                            }
                        }

                    }
                }
            }
            return View(db.Courses.ToList());
        }

        public class MyWebClinet
        {
            private CookieContainer cookies = new CookieContainer(); //cookise的容器在這裡

            public void ClearCookies()  //清除cookise
            {
                cookies = new CookieContainer();
            }

            public HtmlDocument GetPage(string url)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); //向網頁丟request
                request.Method = "GET";

                var NewCookies = new Cookie("over18", "1", "/", "ptt.cc"); //建新cookie規則 名子 值 位置 網域(要有網域 不然add時候出bug)
                cookies.Add(NewCookies);//加進去
                request.CookieContainer = cookies; //直接撿現成的用


                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //接網頁丟回來的respond(大部分是html檔)
                var stream = response.GetResponseStream(); //接 respond 後的資料

                using (var reader = new StreamReader(stream)) //using System.IO; // StreamReader 寫入資料 //讀取用 using
                {
                    string html = reader.ReadToEnd();

                    //一些 ssh 需要的安全規則載入
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //接檔
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    return doc; // over
                }
            }
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DepartmentId,Grade,Number,Class,Name,Teacher,Duration,Credit,Type,Time,Location,Limit,Url,Remark")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepartmentId,Grade,Number,Class,Name,Teacher,Duration,Credit,Type,Time,Location,Limit,Url,Remark")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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

        private bool CheckNameExist(List<string> courseNames, string courseName)
        {
            foreach (var name in courseNames)
            {
                if (courseName == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
