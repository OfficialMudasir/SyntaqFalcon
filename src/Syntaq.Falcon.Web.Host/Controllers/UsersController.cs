using Abp.AspNetCore.Mvc.Authorization;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Storage;
using Abp.BackgroundJobs;

namespace Syntaq.Falcon.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}