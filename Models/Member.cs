using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class Member
    {
        [DisplayName("身分證字號")]
        [RegularExpression(@"^[A-Z]{1}[1-2]{1}\d{8}$", ErrorMessage = "身分證格式錯誤")]
        public string ID { get; set; }

        [DisplayName("密碼")]
        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "長度須為6~30")]
        [RegularExpression(@"^(?!.*[^a-zA-z0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "須包含最少1個大寫字母、最少1個小寫字母、 最少1個數字，不可有其他符號")]
        public string Password { get; set; }

        [DisplayName("確認密碼")]
        [Required]
        [Compare("Password", ErrorMessage = "輸入必須與密碼相同")]
        public string Password2 { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("英文姓名")]
        public string EnglishName { get; set; }

        [DisplayName("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("手機")]
        [Required]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "長度須為10且為數字")]
        public string phoneNumber { get; set; }

        [DisplayName("姓別")]
        public string Gender { get; set; }

        [DisplayName("是否訂閱電子報")]
        public string IsSubscribe { get; set; }

        [DisplayName("生日")]
        public string birthday { get; set; }

        [DisplayName("年紀")]
        public int Age { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("科系")]
        public string Department { get; set; }

        [DisplayName("學校")]
        public string School { get; set; }

        [DisplayName("加入會員時間")]
        public DateTime RegisterTime { get; set; }
    }
}