using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace 阿災.Models
{
    public class ClassChoose
    {
       
        public int num { get; set; }
        [Key]
        public Guid Id { get; set; }
        public string classname { get; set; }
        public string classurl { get; set; }
    }
}