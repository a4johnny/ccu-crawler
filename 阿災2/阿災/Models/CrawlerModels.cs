using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace 阿災.Models
{
    public class Crawler
    {
        [Key]
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string Teacher { set; get; }
        public string Time { set; get; }
        public string Location { set; get; }
        public string Limit { set; get; }
        public string Department { set; get; }
        public int num { set; get; }
    }
}