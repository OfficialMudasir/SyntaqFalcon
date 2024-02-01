using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Projects
{
    public interface IProjectReleasesAppService : IApplicationService
    {
        Task<PagedResultDto<GetProjectReleaseForViewDto>> GetAll(GetAllProjectReleasesInput input);

        Task<GetProjectReleaseForViewDto> GetProjectReleaseForView(Guid id);

        Task<GetProjectReleaseForEditOutput> GetProjectReleaseForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditProjectReleaseDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetProjectReleasesToExcel(GetAllProjectReleasesForExcelInput input);

        Task<PagedResultDto<ProjectReleaseProjectEnvironmentLookupTableDto>> GetAllProjectEnvironmentForLookupTable(GetAllForLookupTableInput input);

        Task<GetProjectReleaseVersionForEditOutput> GetProjectReleaseVersionForEdit(EntityDto<Guid> input);
    }
}