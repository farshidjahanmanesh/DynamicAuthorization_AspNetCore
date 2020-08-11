using System.Collections.Generic;
using System.Linq;

namespace IdentityLearning.Models
{
    public enum persmission
    {
        BasePermission = 1,

        AddPostPermission = 11,
        RemovePostPermission = 12,
        UpdatePostPermission = 13,

        RoleView=20,
        CreateRole=21,
        DeleteRole=22,
        UpdateRole=23,
        AccessLevelRole=24,

        

        UserView=30,
        UpdateUser=31,
        SetRoleTOUser=32,
        DeleteUser=33

    }


    public static class ClaimToPermission
    {
        static Dictionary<string, string> PersianPermissions = new Dictionary<string, string>();
        static ClaimToPermission()
        {
            PersianPermissions.Add("BasePermission", "سطح دسترسی پایه");
            PersianPermissions.Add("AddPostPermission", "اضافه کردن مقاله");
            PersianPermissions.Add("UpdatePostPermission", "حذف مقاله");


            PersianPermissions.Add("RoleView", "دیدن نقش ها");

            PersianPermissions.Add("CreateRole", "ایجاد یک نقش");

            PersianPermissions.Add("DeleteRole", "حذف یک نقش");

            PersianPermissions.Add("UpdateRole", "تغییر یک نقش");

            PersianPermissions.Add("SetRoleTOUser", "افزودن یک نقش به یوزر");
            PersianPermissions.Add("AccessLevelRole", "سطح دسترسی دادن به نقش های مختلف");

            PersianPermissions.Add("UserView", "دیدن یوزر ها");
            PersianPermissions.Add("UpdateUser", "اعمال تغییرات در کاربر");

        }
        public static Dictionary<string, string> GetAllClaims()
        {
            return PersianPermissions;
        }
    }
}
