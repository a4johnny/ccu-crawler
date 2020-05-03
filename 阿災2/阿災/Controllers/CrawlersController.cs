using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using 阿災.Models;

namespace 阿災.Controllers
{
    public class CrawlersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Crawlers
        public ActionResult Index()
        {
            return View(db.Crawlers.ToList());
        }

        [HttpPost] //BeginForm 回傳的要求會在這裡觸發
        [ValidateAntiForgeryToken] //@Html.AntiForgeryToken() 不知其用意
        public ActionResult Index(Crawler crawler)
        {
            if (ModelState.IsValid)
            {
                var clienta = new MyWebClinet();
                //string ClassNum = "4106";
                //using HtmlAgilityPack;
                var count = db.ClassChooses.ToList().Count;
                var num = -1;
                string aaaPlus = "a"; // 不宣在這裡 c# 會中風  未指定的區域變數??????????????
                while (count > 0)
                {
                    count--;
                    num++;
                    int number = 0; // 課程數
                    var StdUrl = "https://kiki.ccu.edu.tw/~ccmisp06/Course/4106.html"; // 比較用  跨領域的網址比較長

                    //現在要爬的系所名稱 和 網址
                    ClassChoose TheOne = db.ClassChooses.Where(p => p.num == num).FirstOrDefault();
                    var url = TheOne.classurl;
                    var type = 1;
                    if (url.Length > StdUrl.Length) // type2 為跨領域課程
                        type = 2;
                    // 判斷是不是碩士生課程和通識 備註欄位置不同
                    var different = 0;
                    if (TheOne.classname.IndexOf("碩", 0) > 0 || TheOne.classname.IndexOf("所", 0) > 0 || TheOne.classname.IndexOf("通識中心", 0) > 0)
                    {
                        different = 1;
                    }
                    //HtmlDocument doc = clienta.GetPage("https://kiki.ccu.edu.tw/~ccmisp06/Course/4106.html");
                    //拿 html 檔
                    HtmlDocument doc = clienta.GetPage(url);
                    ///抓 xpath
                    HtmlNodeCollection list = doc.DocumentNode.SelectNodes("/html/body/center");

                    if (type == 1)
                    {
                        int j = 2;
                        for (int i = 0; i < 300; i++)
                        {   //xpath node 不能加 tbody 不然會crash 原因不明
                            // 通識中心格式不同 多一格向度 向度的資料跟備註放在一起

                            string aaa, aaa2, aaa3, aaa4, aPlus = "a"; // 先宣告 不然在其他 fuction 看不到
                            // 宣告 XML 路徑
                            if (TheOne.classname == "通識中心")  //通識中心前面多了一個 向度欄位  所有 XML 向後退一格
                            {
                                aaaPlus = "table/tr[" + j.ToString() + "]/td[2]"; //向度
                                aaa = "table/tr[" + j.ToString() + "]/td[5]/font"; //課程名稱
                                aaa2 = "table/tr[" + j.ToString() + "]/td[6]/font"; // 老師
                                aaa3 = "table/tr[" + j.ToString() + "]/td[10]/font"; //時間
                                aaa4 = "table/tr[" + j.ToString() + "]/td[11]/font"; // 地點
                            }
                            else
                            {
                                aaa = "table/tr[" + j.ToString() + "]/td[4]/font";
                                aaa2 = "table/tr[" + j.ToString() + "]/td[5]/font";
                                aaa3 = "table/tr[" + j.ToString() + "]/td[9]/font";
                                aaa4 = "table/tr[" + j.ToString() + "]/td[10]/font";
                            }
                            string aaa5 = "temp"; //備註

                            if (different == 1) // 碩士班比學士班多一格學制 different = 1 表示為碩士班課程
                            {
                                aaa5 = "table/tr[" + j.ToString() + "]/td[14]/font";
                            }
                            else
                            {
                                aaa5 = "table/tr[" + j.ToString() + "]/td[13]/font";
                            }

                            
                            if (list[0].SelectSingleNode(aaa) == null) // 抓不到東西 跳出迴圈
                            {
                                i = 1000;
                                //Console.WriteLine("over");
                            }
                            else
                            {   //帶入 xml 抓取資料
                                string a1 = list[0].SelectSingleNode(aaa).InnerText; //名稱
                                string a2 = list[0].SelectSingleNode(aaa2).InnerText; //老師
                                string a3 = list[0].SelectSingleNode(aaa3).InnerText; //時間
                                string a4 = list[0].SelectSingleNode(aaa4).InnerText; //地點
                                string a5 = list[0].SelectSingleNode(aaa5).InnerText; //備註
                                if (TheOne.classname == "通識中心")
                                {
                                    aPlus = list[0].SelectSingleNode(aaaPlus).InnerText; //向度 通識專屬
                                }
                                //Console.WriteLine(a1 + a2 + a3 + a4);

                                Crawler crawler1 = new Crawler();  //寫入db
                                crawler1.Id = Guid.NewGuid();
                                crawler1.Name = a1;
                                crawler1.Teacher = a2;
                                crawler1.Time = a3;
                                crawler1.Location = a4;
                                if (a5 == "")// 備註為空 補上 none
                                {
                                    crawler1.Limit = "none";
                                }
                                else
                                {
                                    crawler1.Limit = a5;
                                }
                                crawler1.Department = TheOne.classname;
                                number++;
                                crawler1.num = number;
                                if (TheOne.classname == "通識中心") //通識中心的向度 補在 備註內 不用在多一個 db 欄位去記
                                {
                                    crawler1.Limit = crawler1.Limit + ",向度:" + aPlus;
                                }
                                db.Crawlers.Add(crawler1);
                                db.SaveChanges();
                                j++;
                            }
                        }
                    }
                    else if (type == 2) // 處理 跨領域課程 基本上都是重複的課程 所以這裡只把它有跨的領域 寫到備註裡
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
                                Crawler temp = db.Crawlers.Where(p => p.Name.Contains(a6)).FirstOrDefault(); //找到課程名稱相同的人
                                temp.Limit = temp.Limit + ",跨領域課程:" + TheOne.classname; //寫進備註
                                db.SaveChanges();
                            }
                            else // 跳出迴圈
                            {
                                i = 31;
                            }
                        }

                    }
                }
            }
            return View(db.Crawlers.ToList());
            //return View();
        }
        // 寫入 cookise 部分
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
        // GET: Crawlers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Crawler crawler = db.Crawlers.Find(id);
            if (crawler == null)
            {
                return HttpNotFound();
            }
            return View(crawler);
        }

        // GET: Crawlers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Crawlers/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Teacher,Time,Location")] Crawler crawler)
        {
            if (ModelState.IsValid)
            {
                crawler.Id = Guid.NewGuid();
                db.Crawlers.Add(crawler);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(crawler);
        }

        // GET: Crawlers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Crawler crawler = db.Crawlers.Find(id);
            if (crawler == null)
            {
                return HttpNotFound();
            }
            return View(crawler);
        }

        // POST: Crawlers/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Teacher,Time,Location")] Crawler crawler)
        {
            if (ModelState.IsValid)
            {
                db.Entry(crawler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(crawler);
        }

        // GET: Crawlers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Crawler crawler = db.Crawlers.Find(id);
            if (crawler == null)
            {
                return HttpNotFound();
            }
            return View(crawler);
        }

        // 這個 function 沒反應
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAll(Guid? id)
        {
            Crawler crawler = new Crawler();
            while (db.Crawlers.Where(p => p.Id != null) != null)
            {
                db.Crawlers.RemoveRange(db.Crawlers.Where(x => x.Id != null));
            }
            db.SaveChanges();
            return RedirectToAction("Create");
        }


        // POST: Crawlers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id, string action)
        {
            if (action == "Delete") //單一刪除
            {
                Crawler crawler = db.Crawlers.Find(id);
                db.Crawlers.Remove(crawler);
                db.SaveChanges();
            }
            else if (action == "DeleteAll")//全部刪除
            {   
                Crawler crawler = new Crawler();
                int count = db.Crawlers.ToList().Count; //求出共有幾筆
                while (count != 0) //砍到沒有為止
                {
                    Crawler crawler1 = db.Crawlers.Where(x => x.Id != null).FirstOrDefault(); // id 非null就砍
                    db.Crawlers.Remove(crawler1);
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
