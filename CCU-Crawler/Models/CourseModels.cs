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
}