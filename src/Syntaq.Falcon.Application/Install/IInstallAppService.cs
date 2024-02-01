using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.Install.Dto;

namespace Syntaq.Falcon.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}