using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization.Permissions.Dto;

namespace Syntaq.Falcon.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
