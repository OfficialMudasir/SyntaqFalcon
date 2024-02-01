using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Syntaq.Falcon.Files;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Web.Controllers
{
    [AbpMvcAuthorize]
    [DisableAuditing]
    public class FilesController : FilesControllerBase
    {
        public FilesController(ITempFileCacheManager tempFileCacheManager, FilesManager filesManager) :
            base(tempFileCacheManager, filesManager)
        {
        }
    }
}
