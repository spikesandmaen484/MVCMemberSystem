using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Exam.Models
{
    public class PersonalInfo
    {
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("加入會員時間")]
        public DateTime RegisterTime { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("英文姓名")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "只能輸入英文字母")]
        public string EnglishName { get; set; }

        [DisplayName("手機")]
        [Required]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "長度須為10且為數字")]
        public string phoneNumber { get; set; }

        [DisplayName("姓別")]
        public string Gender { get; set; }

        [DisplayName("生日")]
        [RegularExpression(@"\b(?<year>\d{2,4})/(?<month>\d{1,2})/(?<day>\d{1,2})\b", ErrorMessage = "生日格式錯誤")]
        public string birthday { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        public string photo { get; set; }
    }

    public enum Gender 
    {
        Male,
        Female
    }
}