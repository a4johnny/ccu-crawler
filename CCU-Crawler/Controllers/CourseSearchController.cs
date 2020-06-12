using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using CCU_Crawler.Models;
using Microsoft.Ajax.Utilities;

namespace CCU_Crawler.Controllers
{
    public class CourseSearchController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseSearch
        public ActionResult Index(SearchCourse searchCourse)
        {
            if (searchCourse is object)
            {
                var courses = new List<Course>();
                if (!searchCourse.DepartmentName.IsNullOrWhiteSpace())
                {
                    ViewData["DepartmentName"] = searchCourse.DepartmentName;
                    var currentDepartment = db.Departments.Where(department => department.Name.Contains(searchCourse.DepartmentName)).FirstOrDefault();
                    if (currentDepartment is object)
                    {
                        var integerDepartmentId = db.Departments.Where(department => department.Name.Contains(searchCourse.DepartmentName)).FirstOrDefault().Id;
                        courses = db.Courses.Where(course => course.DepartmentId == integerDepartmentId).ToList();
                        Debug.WriteLine(courses.Count);
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
                                     select new CourseToView()
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
                switch (searchCourse.OrderType)
                {
                    case 0:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.Popularity).ToList();
                        break;
                    case 1:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.CourseName.Length).ToList();
                        break;
                    case 2:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.Credit).ToList();
                        break;
                    case -1:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.Popularity).ToList();
                        break;
                    case -2:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.CourseName.Length).ToList();
                        break;
                    case -3:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.Credit).ToList();
                        break;
                    default:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.Popularity).ToList();
                        break;

                }
                return PartialView(coursesToView);
            }
            else
            {
                ViewData["DepartmentName"] = "";
                ViewData["Grade"] = "";
                ViewData["Name"] = "";
                ViewData["Teacher"] = "";
                var courses = db.Courses.ToList();
                var coursesToView = (from course in courses
                                     join department in db.Departments on course.DepartmentId equals department.Id
                                     select new CourseToView()
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
                switch (searchCourse.OrderType)
                {
                    case 0:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.Popularity).ToList();
                        break;
                    case 1:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.CourseName.Length).ToList();
                        break;
                    case 2:
                        coursesToView = coursesToView.OrderByDescending(courseToView => courseToView.Credit).ToList();
                        break;
                    case -1:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.Popularity).ToList();
                        break;
                    case -2:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.CourseName.Length).ToList();
                        break;
                    case -3:
                        coursesToView = coursesToView.OrderBy(courseToView => courseToView.Credit).ToList();
                        break;
                }
                return PartialView(coursesToView);
            }
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search([Bind(Include = "DepartmentName,Grade,Name,Teacher")] SearchCourse searchCourse)
        {
            return RedirectToAction("Index", searchCourse);
        }

        public ActionResult HighPopularity()
        {
            var CommentList = new List<Course>();
            CommentList = db.Courses.Where(p => p.Popularity > 0)
                                    .OrderByDescending(x => x.Popularity)
                                    .ToList();
                                    //.GetRange(0, 3);
            if (CommentList.Count > 2)
                CommentList = CommentList.GetRange(0, 3);
            //CommentList = CommentList.GetRange(0, 3).ToList();
            return PartialView(CommentList);
        }
    }
}
