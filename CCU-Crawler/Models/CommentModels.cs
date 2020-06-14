using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCU_Crawler.Models
{
    public class Comment
    {   
        [Key]
        public int Id { get; set; }
        public int GoodOrBad { get; set; }
        public int CommentId { get; set; }
        public int CourseId { get; set; }
        public string User { get; set; }
    }
}