using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.Editions.Dto;
using Syntaq.Falcon.MultiTenancy.Dto;

namespace Syntaq.Falcon.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}