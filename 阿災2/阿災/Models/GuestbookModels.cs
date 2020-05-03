using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace 阿災.Models
{
    public class Guestbook
    {   
        [Key] //作為索引
        public Guid Id { get; set; } //可以取得(get)也可以寫入(set) guid 產生唯一數值
        public string Contend { get; set; }
        public DateTime? Created { get; set; } //用datatime命名 可能要求時間格式  ?允許空值
        public ApplicationUser User { get; set; }
    }
}