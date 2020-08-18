using System.Collections.Generic;

namespace SharedServices.Models.IdentityModels
{
    public static class ClaimToPermission
    {
        static Dictionary<string, string> PersianPermissions = new Dictionary<string, string>();
        static ClaimToPermission()
        {
            PersianPermissions.Add("BasePermission", "سطح دسترسی پایه");


            PersianPermissions.Add("RoleView", "دیدن نقش ها");
            PersianPermissions.Add("CreateRole", "ایجاد یک نقش");
            PersianPermissions.Add("DeleteRole", "حذف یک نقش");
            PersianPermissions.Add("UpdateRole", "تغییر یک نقش");

            

            PersianPermissions.Add("UserView", "دیدن یوزر ها");
            PersianPermissions.Add("UpdateUser", "اعمال تغییرات در کاربر");
            PersianPermissions.Add("SetRoleTOUser", "افزودن یک نقش به یوزر");
            PersianPermissions.Add("AccessLevelRole", "سطح دسترسی دادن به نقش های مختلف");


            PersianPermissions.Add("ViewerVisitor", "نمایش آمار بازدید کاربران (گراف های صفحه ادمین )");
            PersianPermissions.Add("DeviceChecker", "نمایش آمار بازدید گوشی ها (گراف های مربوط به صفحه ادمین )");
            PersianPermissions.Add("BrowserVisitor", "نمایش آمار مرورگرهای کاربران (گراف های صفحه ادمین )");
        }
        public static Dictionary<string, string> GetAllClaims()
        {
            return PersianPermissions;
        }
    }
}
