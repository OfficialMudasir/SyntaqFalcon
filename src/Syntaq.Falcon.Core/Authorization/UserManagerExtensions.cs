using System.Threading.Tasks;
using Abp.Authorization.Users;
using Syntaq.Falcon.Authorization.Users;

namespace Syntaq.Falcon.Authorization
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetAdminAsync(this UserManager userManager)
        {
            return await userManager.FindByNameAsync(AbpUserBase.AdminUserName);
        }
    }
}
