using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam.Models
{
    public class Activity
    {
        [DisplayName("名稱")]
        [Required]
        public string Name { get; set; }

        [DisplayName("日期")]
        [Required]
        [RegularExpression(@"\b(?<year>\d{2,4})/(?<month>\d{1,2})/(?<day>\d{1,2})\b", ErrorMessage = "日期格式錯誤")]
        public string ActivityDateStart { get; set; }

        [Required]
        [RegularExpression(@"\b(?<year>\d{2,4})/(?<month>\d{1,2})/(?<day>\d{1,2})\b", ErrorMessage = "日期格式錯誤")]
        public string ActivityDateEnd { get; set; }

        [DisplayName("地點")]
        public string Address { get; set; }

        [DisplayName("費用")]
        [RegularExpression(@"^\d+$", ErrorMessage = "須為數字")]
        public decimal Fee { get; set; }
    }
}