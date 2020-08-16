using IdentityLearning.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    [CheckXss]
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Compare("Password", ErrorMessage = "{0} مطابقت ندارد")]
        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
        public string Email { get; set; }

    }
}
