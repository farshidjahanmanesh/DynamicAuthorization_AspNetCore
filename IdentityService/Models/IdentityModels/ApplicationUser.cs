using Microsoft.AspNetCore.Identity;

namespace SharedServices.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public string AboutMe { get; set; }
        public string PersianName { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool IsExternalLogin { get; set; }

    }
}
