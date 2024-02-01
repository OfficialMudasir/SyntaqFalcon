using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Projects
{
    public interface IProjectTenantsAppService : IApplicationService
    {
        Task<PagedResultDto<GetProjectTenantForViewDto>> GetAll(GetAllProjectTenantsInput input);
        Task<PagedResultDto<GetProjectTenantForViewDto>> GetAllForTenant(GetAllProjectTenantsInput input);

        Task<GetProjectTenantForViewDto> GetProjectTenantForView(Guid id);

        Task<GetProjectTenantForEditOutput> GetProjectTenantForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditProjectTenantDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetProjectTenantsToExcel(GetAllProjectTenantsForExcelInput input);

        Task<PagedResultDto<ProjectTenantProjectEnvironmentLookupTableDto>> GetAllProjectEnvironmentForLookupTable(GetAllForLookupTableInput input);

    }
}