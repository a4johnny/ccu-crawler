using System.ComponentModel.DataAnnotations;

namespace CCU_Crawler.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public int CollegeId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}