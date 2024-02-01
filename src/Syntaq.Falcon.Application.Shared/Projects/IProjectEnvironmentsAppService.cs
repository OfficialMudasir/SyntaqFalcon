using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Projects
{
    public interface IProjectEnvironmentsAppService : IApplicationService
    {
        Task<PagedResultDto<GetProjectEnvironmentForViewDto>> GetAll(GetAllProjectEnvironmentsInput input);

        Task<GetProjectEnvironmentForViewDto> GetProjectEnvironmentForView(Guid id);

        Task<GetProjectEnvironmentForEditOutput> GetProjectEnvironmentForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditProjectEnvironmentDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetProjectEnvironmentsToExcel(GetAllProjectEnvironmentsForExcelInput input);

    }
}