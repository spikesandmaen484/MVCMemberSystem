using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Exam.Models
{
    public class ActivityRead
    {
        [DisplayName("名稱")]
        public string Name { get; set; }

        [DisplayName("日期")]
        public string ActivityDateStart { get; set; }

        public string ActivityDateEnd { get; set; }
    }
}