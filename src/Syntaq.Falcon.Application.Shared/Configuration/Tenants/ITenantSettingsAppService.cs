using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.Configuration.Tenants.Dto;

namespace Syntaq.Falcon.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
