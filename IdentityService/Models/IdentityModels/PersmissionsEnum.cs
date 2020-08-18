using System.Linq;

namespace SharedServices.Models.IdentityModels
{

    public enum PersmissionsEnum
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
}
