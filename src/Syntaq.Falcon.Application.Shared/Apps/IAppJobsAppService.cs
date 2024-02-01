using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Apps
{
    public interface IAppJobsAppService : IApplicationService 
    {
        //Task<PagedResultDto<GetAppJobForView>> GetAll(GetAllAppJobsInput input);

        Task<PagedResultDto<GetAppJobForView>> GetJobsByAppId(EntityDto<Guid> input);

        Task<GetAppJobForEditOutput> GetAppJobForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditAppJobDto input);

		Task Delete(EntityDto<Guid> input);

		//Task<FileDto> GetAppJobsToExcel(GetAllAppJobsForExcelInput input);
        		
		//Task<PagedResultDto<AppLookupTableDto>> GetAllAppForLookupTable(GetAllForLookupTableInput input);
		
    }
}