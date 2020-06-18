using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using CCU_Crawler.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace CCU_Crawler.Controllers
{
    public class CourseSearchController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private SearchCourse lastSearchSetting;
        private const int PAGE_SIZE = 20;
        public ActionResult Index(int? page)
        {
            ViewData["Page"] = page == null ? 1 : page;
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = "DepartmentName,Grade,Name,Teacher")] SearchCourse searchCourse)
        {
            ViewData["Page"] = searchCourse is object ? searchCourse.Page : 1;
            return RedirectToAction("Search", searchCourse);
        }
        public ActionResult PageList(SearchCourse searchCourse)
        {
            return PartialView(searchCourse);
        }
        public ActionResult Search(SearchCourse searchCourse)
        {
            if (searchCourse is object)
            {
                if (lastSearchSetting is object)
                {
                    if (searchCourse.DepartmentName != lastSearchSetting.DepartmentName || searchCourse.Grade != lastSearchSetting.Grade || searchCourse.Name != lastSearchSetting.Name)
                    {
                        searchCourse.Page = 1;
                    }
                }
                lastSearchSetting = searchCourse;
                searchCourse.Page = searchCourse.Page == 0 ? 1 : searchCourse.Page;
                var courses = new List<Course>();
                if (!searchCourse.DepartmentName.IsNullOrWhiteSpace())
                {
                    ViewData["DepartmentName"] = searchCourse.DepartmentName;
                    var currentDepartment = db.Departments.Where(department => department.Name.Contains(searchCourse.DepartmentName)).FirstOrDefault();
                    if (currentDepartment is object)
                    {
                        var integerDepartmentId = db.Departments.Where(department => department.Name.Contains(searchCourse.DepartmentName)).FirstOrDefault().Id;
                        courses = db.Courses.Where(course => course.DepartmentId == integerDepartmentId).ToList();
                    }
                }
                else
                {
                    ViewData["DepartmentName"] = "";
                    courses = db.Courses.ToList();
                }
                if (!searchCourse.Grade.IsNullOrWhiteSpace() && searchCourse.Grade != string.Empty)
                {
                    ViewData["Grade"] = searchCourse.Grade;
                    var integerGrade = int.Parse(searchCourse.Grade);
                    if (courses.Count == 0)
                    {
                        courses = db.Courses.Where(course => course.Grade == integerGrade).ToList();
                    }
                    else
                    {
                        courses = courses.Where(course => course.Grade == integerGrade).ToList();
                    }
                }
                else
                {
                    ViewData["Grade"] = "";
                }
                if (!searchCourse.Name.IsNullOrWhiteSpace() && searchCourse.Name != string.Empty)
                {
                    ViewData["Name"] = searchCourse.Name;
                    if (courses.Count == 0)
                    {
                        courses = db.Courses.Where(course => course.Name.Contains(searchCourse.Name)).ToList();
                    }
                    else
                    {
                        courses = courses.Where(course => course.Name.Contains(searchCourse.Name)).ToList();
                    }
                }
                else
                {
                    ViewData["Name"] = "";
                }
                if (!searchCourse.Teacher.IsNullOrWhiteSpace() && searchCourse.Teacher != string.Empty)
                {
                    ViewData["Teacher"] = searchCourse.Teacher;
                    if (courses.Count == 0)
                    {
                        courses = db.Courses.Where(course => course.Teacher.Contains(searchCourse.Teacher)).ToList();
                    }
                    else
                    {
                        courses = courses.Where(course => course.Teacher.Contains(searchCourse.Teacher)).ToList();
                    }
                }
                else
                {
                    ViewData["Teacher"] = "";
                }
                var coursesToView = (from course in courses
                                     join department in db.Departments on course.DepartmentId equals department.Id
                                     select new JointSearchCourse()
                                     {
                                         Id = course.Id,
                                         DepartmentName = department.Name,
                                         Grade = course.Grade,
                                         CourseName = course.Name,
                                         Teacher = course.Teacher,
                                         Duration = course.Duration,
                                         Credit = course.Credit,
                                         Type = course.Type,
                                         Time = course.Time,
                                         Location = course.Location,
                                         Limit = course.Limit,
                                         Url = course.Url,
                                         Remark = course.Remark,
                                         Popularity = course.Popularity
                                     }).ToList();
                ViewData["OrderType"] = searchCourse.OrderType;
                ViewData["Page"] = searchCourse.Page;
                Debug.WriteLine(searchCourse.Page);
                return PartialView(CourseToViewOrder(coursesToView.ToPagedList(searchCourse.Page, PAGE_SIZE).ToList(), searchCourse));
            }
            else
            {
                ViewData["DepartmentName"] = "";
                ViewData["Grade"] = "";
                ViewData["Name"] = "";
                ViewData["Teacher"] = "";
                ViewData["Page"] = 1;
                var courses = db.Courses.ToList();
                var coursesToView = (from course in courses
                                     join department in db.Departments on course.DepartmentId equals department.Id
                                     select new JointSearchCourse()
                                     {
                                         Id = course.Id,
                                         DepartmentName = department.Name,
                                         Grade = course.Grade,
                                         CourseName = course.Name,
                                         Teacher = course.Teacher,
                                         Duration = course.Duration,
                                         Credit = course.Credit,
                                         Type = course.Type,
                                         Time = course.Time,
                                         Location = course.Location,
                                         Limit = course.Limit,
                                         Url = course.Url,
                                         Remark = course.Remark
                                     }).ToList();
                return PartialView(CourseToViewOrder(coursesToView.ToPagedList(1, PAGE_SIZE).ToList(), searchCourse));
            }
        }
        private JointSearchCourseToView CourseToViewOrder(List<JointSearchCourse> jointSearchCourseList, SearchCourse searchCourse)
        {
            Debug.WriteLine(jointSearchCourseList.Count);
            switch (searchCourse.OrderType)
            {
                case 1:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderByDescending(courseToView => courseToView.CourseName.Length).ToList(),
                        SearchCourse = searchCourse
                    };
                case 2:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderByDescending(courseToView => courseToView.Credit).ToList(),
                        SearchCourse = searchCourse
                    };
                case -1:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderBy(courseToView => courseToView.Popularity).ToList(),
                        SearchCourse = searchCourse
                    };
                case -2:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderBy(courseToView => courseToView.CourseName.Length).ToList(),
                        SearchCourse = searchCourse
                    };
                case -3:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderBy(courseToView => courseToView.Credit).ToList(),
                        SearchCourse = searchCourse
                    };
                default:
                    return new JointSearchCourseToView
                    {
                        JointSearchCourseList = jointSearchCourseList.OrderByDescending(courseToView => courseToView.Popularity).ToList(),
                        SearchCourse = searchCourse
                    };
            }
        }
    }
}