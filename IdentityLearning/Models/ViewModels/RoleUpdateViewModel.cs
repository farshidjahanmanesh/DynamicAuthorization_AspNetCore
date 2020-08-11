using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    public class RoleUpdateViewModel
    {
        [Required]
        public string RoleId { get; set; }

        [DisplayName("نام قدیم نقش")]
        public string OldRoleName { get; set; }
        [Required(ErrorMessage = "لطفا نام نقش را وارد کنید")]
        [DisplayName("نام جدید نقش")]
        [RegularExpression("[a-zA-Z]+", ErrorMessage = "لطفا فقط از حروف انگلیسی برای نام نقش استفاده کنید")]
        public string NewRoleName { get; set; }
    }
}
