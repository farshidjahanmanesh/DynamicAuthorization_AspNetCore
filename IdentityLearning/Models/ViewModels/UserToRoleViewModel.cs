using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityLearning.Models.ViewModels
{
    public class UserToRoleViewModel
    {
        [Required]
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<SelectListItem> RoleIn { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> RoleOut { get; set; } = new List<SelectListItem>();

    }
}
