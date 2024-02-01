using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using static Syntaq.Falcon.Projects.ProjectDeploymentEnums;

namespace Syntaq.Falcon.Projects
{
    public interface IProjectDeploymentsAppService : IApplicationService
    {
        Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAll(GetAllProjectDeploymentsInput input);
        Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAllForRelease(GetAllProjectDeploymentsInput input);
        Task<PagedResultDto<GetProjectDeploymentForViewDto>> GetAllByProject(GetAllProjectDeploymentsInput input);

        Task<GetProjectDeploymentForViewDto> GetProjectDeploymentForView(Guid id);

        Task<GetProjectDeploymentForEditOutput> GetProjectDeploymentForEdit(EntityDto<Guid> input);

        Task SetType(Guid Id, int type);
        Task SetEnabled(Guid Id, bool enabled);

        Task CreateOrEdit(CreateOrEditProjectDeploymentDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetProjectDeploymentsToExcel(GetAllProjectDeploymentsForExcelInput input);

        Task<PagedResultDto<ProjectDeploymentProjectReleaseLookupTableDto>> GetAllProjectReleaseForLookupTable(GetAllForLookupTableInput input);

    }
}