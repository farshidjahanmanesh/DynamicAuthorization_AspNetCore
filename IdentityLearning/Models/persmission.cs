using System.Collections.Generic;
using System.Linq;

namespace IdentityLearning.Models
{
    public enum graphPermission
    {
        ViewerVisitor,
        DeviceChecker,
        BrowserVisitor
    }

    public enum persmission
    {
        BasePermission = 1,

        RoleView=20,
        CreateRole=21,
        DeleteRole=22,
        UpdateRole=23,
        AccessLevelRole=24,

        

        UserView=30,
        UpdateUser=31,
        SetRoleTOUser=32,
        DeleteUser=33,


        ViewerVisitor=60,
        DeviceChecker=61,
        BrowserVisitor=62
    }


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
