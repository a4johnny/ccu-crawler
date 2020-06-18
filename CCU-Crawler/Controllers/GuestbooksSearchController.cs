using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CCU_Crawler.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace CCU_Crawler.Controllers
{
    public class GuestbooksSearchController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private SearchGuestbook lastSearchSetting;
        private const int PAGE_SIZE = 10;
        public ActionResult Index(int? page)
        {
            ViewData["Page"] = page == null ? 1 : page;
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = "Keyword,Score,Call,Sign,Group")] SearchGuestbook searchGuestbook)
        {
            return RedirectToAction("Search", searchGuestbook);
        }
        public ActionResult PageList(SearchGuestbook searchGuestbook)
        {
            return PartialView(searchGuestbook);
        }
        public ActionResult Search(SearchGuestbook searchGuestbook)
        {
            if (searchGuestbook is object)
            {
                if (lastSearchSetting is object)
                {
                    if (searchGuestbook.Keyword != lastSearchSetting.Keyword || searchGuestbook.Score != lastSearchSetting.Score)
                    {
                        Debug.WriteLine("G");
                        searchGuestbook.Page = 1;
                    }
                }
                lastSearchSetting = searchGuestbook;
                searchGuestbook.Page = searchGuestbook.Page == 0 ? 1 : searchGuestbook.Page;
                var guestbooks = new List<Guestbook>();
                if (!searchGuestbook.Keyword.IsNullOrWhiteSpace())
                {
                    ViewData["Keyword"] = searchGuestbook.Keyword;
                    guestbooks = db.Guestbooks.Where(guestbook => guestbook.Content.Contains(searchGuestbook.Keyword)).ToList();
                }
                else
                {
                    ViewData["Keyword"] = "";
                    guestbooks = db.Guestbooks.ToList();
                }
                if (!searchGuestbook.Score.IsNullOrWhiteSpace() && searchGuestbook.Score != string.Empty)
                {
                    ViewData["Score"] = searchGuestbook.Score;
                    var integerScore = int.Parse(searchGuestbook.Score);
                    if (guestbooks.Count == 0)
                    {
                        guestbooks = db.Guestbooks.Where(guestbook => guestbook.Score == integerScore).ToList();
                    }
                    else
                    {
                        guestbooks = guestbooks.Where(guestbook => guestbook.Score == integerScore).ToList();
                    }
                }
                else
                {
                    ViewData["Score"] = "";
                }
                if (!searchGuestbook.Call.IsNullOrWhiteSpace() && searchGuestbook.Call != string.Empty)
                {
                    ViewData["Call"] = searchGuestbook.Call;
                    var integerCall = int.Parse(searchGuestbook.Call);
                    if (guestbooks.Count == 0)
                    {
                        guestbooks = db.Guestbooks.Where(guestbook => guestbook.Score == integerCall).ToList();
                    }
                    else
                    {
                        guestbooks = guestbooks.Where(guestbook => guestbook.Score == integerCall).ToList();
                    }
                }
                else
                {
                    ViewData["Call"] = "";
                }
                if (!searchGuestbook.Sign.IsNullOrWhiteSpace() && searchGuestbook.Sign != string.Empty)
                {
                    ViewData["Sign"] = searchGuestbook.Sign;
                    var integerSign = int.Parse(searchGuestbook.Sign);
                    if (guestbooks.Count == 0)
                    {
                        guestbooks = db.Guestbooks.Where(guestbook => guestbook.Score == integerSign).ToList();
                    }
                    else
                    {
                        guestbooks = guestbooks.Where(guestbook => guestbook.Score == integerSign).ToList();
                    }
                }
                else
                {
                    ViewData["Sign"] = "";
                }
                if (!searchGuestbook.Sign.IsNullOrWhiteSpace() && searchGuestbook.Sign != string.Empty)
                {
                    ViewData["Group"] = searchGuestbook.Group;
                    var integerGroup = int.Parse(searchGuestbook.Group);
                    if (guestbooks.Count == 0)
                    {
                        guestbooks = db.Guestbooks.Where(guestbook => guestbook.Score == integerGroup).ToList();
                    }
                    else
                    {
                        guestbooks = guestbooks.Where(guestbook => guestbook.Score == integerGroup).ToList();
                    }
                }
                else
                {
                    ViewData["Group"] = "";
                }
                ViewData["OrderType"] = searchGuestbook.OrderType;
                ViewData["Page"] = searchGuestbook.Page;
                return PartialView(GuestbooksOrder(guestbooks.ToPagedList(searchGuestbook.Page, PAGE_SIZE).ToList(), searchGuestbook));
            }
            else
            {
                ViewData["Keyword"] = "";
                ViewData["Score"] = "";
                ViewData["Call"] = "";
                ViewData["Sign"] = "";
                ViewData["Group"] = "";
                ViewData["Page"] = 1;
                return PartialView(GuestbooksOrder(db.Guestbooks.ToPagedList(1, PAGE_SIZE).ToList(), searchGuestbook));
            }
        }
        private GuestbookToView GuestbooksOrder(List<Guestbook> guestbooks, SearchGuestbook searchGuestbook)
        {
            switch (searchGuestbook.OrderType)
            {
                case 1:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderByDescending(guestbook => guestbook.Call).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case 2:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderByDescending(guestbook => guestbook.Sign).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case 3:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderByDescending(guestbook => guestbook.Group).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case -1:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderBy(guestbook => guestbook.Score).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case -2:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderBy(guestbook => guestbook.Call).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case -3:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderBy(guestbook => guestbook.Sign).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                case -4:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderBy(guestbook => guestbook.Group).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
                default:
                    return new GuestbookToView
                    {
                        GuestbookList = guestbooks.OrderByDescending(guestbook => guestbook.Score).ToList(),
                        SearchGuestbook = searchGuestbook
                    };
            }
        }
    }
}