using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        [EmailAddress(ErrorMessage = " {0} وارد شده معتبر نیست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="ایمیل")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }


        [Display(Name = "درباره من")]
        public string AboutMe { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام کاربری من")]
        public string PersianName { get; set; }
    }
}
