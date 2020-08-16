using IdentityLearning.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    [CheckXss]
    public class ChangePasswrodViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [Compare("Password", ErrorMessage ="{0} مطابقت ندارد")]
        [Display(Name ="تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="رمز عب")]
        public string OldPassword { get; set; }

        public bool IsExternal { get; set; }
    }
}
