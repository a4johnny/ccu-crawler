using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CCU_Crawler.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int Grade { get; set; }
        public string Number { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Duration { get; set; }
        public int Credit { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public int Limit { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
    }
    public class SearchCourse
    {
        public string DepartmentName { get; set; }
        public string Grade { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
    }
    public class CourseToView
    {
        public int Id { get; set; }
        [DisplayName("系所名稱")]
        public string DepartmentName { get; set; }
        [DisplayName("課程年級")]
        public int Grade { get; set; }
        [DisplayName("課程名稱")]
        public string CourseName { get; set; }
        [DisplayName("任教老師")]
        public string Teacher { get; set; }
        public string Duration { get; set; }
        [DisplayName("學分數")]
        public int Credit { get; set; }
        [DisplayName("必/選")]
        public string Type { get; set; }
        [DisplayName("上課時間")]
        public string Time { get; set; }
        [DisplayName("上課地點")]
        public string Location { get; set; }
        public int Limit { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
    }
}