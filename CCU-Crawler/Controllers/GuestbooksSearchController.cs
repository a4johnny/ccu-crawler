using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCU_Crawler.Models;
using Microsoft.Ajax.Utilities;

namespace CCU_Crawler.Controllers
{
    public class GuestbooksSearchController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: GuestbooksSearch
        public ActionResult Index(SearchGuestbook searchGuestbook)
        {
            if (searchGuestbook is object)
            {
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
                return PartialView(GuestbooksOrder(guestbooks, searchGuestbook.OrderType));
            }
            else
            {
                ViewData["Keyword"] = "";
                ViewData["Score"] = "";
                ViewData["Call"] = "";
                ViewData["Sign"] = "";
                ViewData["Group"] = "";
                return PartialView(GuestbooksOrder(db.Guestbooks.ToList(), searchGuestbook.OrderType));
            }
        }
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search([Bind(Include = "Keyword,Score,Call,Sign,Group")] SearchGuestbook searchGuestbook)
        {
            return RedirectToAction("Index", searchGuestbook);
        }
        private List<Guestbook> GuestbooksOrder(List<Guestbook> guestbooks, int orderType)
        {
            switch (orderType)
            {
                case 1:
                    return guestbooks.OrderByDescending(guestbook => guestbook.Call).ToList();
                case 2:
                    return guestbooks.OrderByDescending(guestbook => guestbook.Sign).ToList();
                case 3:
                    return guestbooks.OrderByDescending(guestbook => guestbook.Group).ToList();
                case -1:
                    return guestbooks.OrderBy(guestbook => guestbook.Score).ToList();
                case -2:
                    return guestbooks.OrderBy(guestbook => guestbook.Call).ToList();
                case -3:
                    return guestbooks.OrderBy(guestbook => guestbook.Sign).ToList();
                case -4:
                    return guestbooks.OrderBy(guestbook => guestbook.Group).ToList();
                default:
                    return guestbooks.OrderByDescending(guestbook => guestbook.Score).ToList();
            }
        }
    }
}