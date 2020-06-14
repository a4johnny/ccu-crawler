using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCU_Crawler.Models
{
    public class Guestbook
    {
        [Key]
        public int Id { get; set; }
        public int Score { get; set; }
        public string Content { get; set; }
        public int? Call { get; set; }
        public int? Sign { get; set; }
        public int? Group { get; set; }
        public string Infomation { get; set;  }

        public int good { get; set; }
        public int bad { get; set; }

        public int CourceId { get; set; }
        public DateTime? DateTime { get; set; }
    }
}