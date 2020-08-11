using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="لطفا ایمیل را وارد کنید")]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده نامعتبر است")]
        [DisplayName("ایمیل")]
        public string Email { get; set; }
        [Required(ErrorMessage ="لطفا رمز عبور را وارد کنید")]
        [DisplayName("رمز عبور")]
        public string Password { get; set; }
        [DisplayName("مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
