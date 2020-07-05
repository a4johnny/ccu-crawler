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
                        //Debug.WriteLine(courses.Count);
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
                return PartialView(CourseToViewOrder(coursesToView, searchCourse.OrderType));
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
                return PartialView(CourseToViewOrder(coursesToView, searchCourse.OrderType));
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
        private List<CourseToView> CourseToViewOrder(List<CourseToView> coursesToView, int orderType)
        {
            switch (orderType)
            {
                case 1:
                    return coursesToView.OrderByDescending(courseToView => courseToView.CourseName.Length).ToList();
                case 2:
                    return coursesToView.OrderByDescending(courseToView => courseToView.Credit).ToList();
                case -1:
                    return coursesToView.OrderBy(courseToView => courseToView.Popularity).ToList();
                case -2:
                    return coursesToView.OrderBy(courseToView => courseToView.CourseName.Length).ToList();
                case -3:
                    return coursesToView.OrderBy(courseToView => courseToView.Credit).ToList();
                default:
                    return coursesToView.OrderByDescending(courseToView => courseToView.Popularity).ToList();
            }
        }
    }
}