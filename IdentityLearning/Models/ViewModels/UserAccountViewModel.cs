using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    public class UserAccountViewModel
    {
        [Required(ErrorMessage ="لطفا {0} خود را وارد کنید")]
        [EmailAddress]
        [Display(Name ="ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        [Display(Name ="رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        [Compare("Password",ErrorMessage ="رمز عبور شما همخوانی ندارد")]
        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "لطفا {0} خود را وارد کنید")]
        public string PersianName { get; set; }

    }
}
