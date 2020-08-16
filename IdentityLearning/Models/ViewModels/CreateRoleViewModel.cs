using IdentityLearning.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    [CheckXss]
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage ="لطفا نام نقش را وارد کنید")]
        [DisplayName("نام نقش")]
        [RegularExpression("[a-zA-Z]+",ErrorMessage ="لطفا فقط از حروف انگلیسی برای نام نقش استفاده کنید")]
        public string RoleName { get; set; }
        public List<SelectListItem> PermissionIn { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PermissionOut { get; set; } = new List<SelectListItem>();

    }
}
