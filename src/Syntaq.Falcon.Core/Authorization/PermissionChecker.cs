using Abp.Authorization;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;

namespace Syntaq.Falcon.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
