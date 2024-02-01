using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.Configuration.Host.Dto;

namespace Syntaq.Falcon.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);

        Task<string> CheckIfAnyActiveUserAcceptanceTypes();
    }
}
