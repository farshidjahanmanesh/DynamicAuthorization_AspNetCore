using IdentityLearning.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models.ViewModels
{
    [CheckXss]
    public class UserUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        [EmailAddress(ErrorMessage = " {0} وارد شده معتبر نیست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name ="ایمیل")]
        public string Email { get; set; }
        
        [RegularPhoneNumber(ErrorMessage ="شماره موبایل نوشته شده صحیح نیست.")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "شماره موبایل")]
        
        public string PhoneNumber { get; set; }


        [Display(Name = "درباره من")]
        public string AboutMe { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام کاربری من")]
        public string PersianName { get; set; }


        [Display(Name ="تصویر کاربری")]
        [ImageRequired(ErrorMessage ="لطفا فقط تصویر انتخاب کنید.")]
        [CheckImageSize(3,ErrorMessage ="حداکثر حجم فایل تصویر سه مگابایت میباشد.")]
        public IFormFile ProfileImage { get; set; }
        public string ProfileImageUrl { get; set; }

    }
}
