using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class LoginValid
    {
        [DisplayName("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("密碼")]
        [Required]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "長度須為6~30")]
        [RegularExpression(@"^(?!.*[^a-zA-z0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "須包含最少1個大寫字母、最少1個小寫字母、 最少1個數字，不可有其他符號")]
        public string Password { get; set; }
    }
}