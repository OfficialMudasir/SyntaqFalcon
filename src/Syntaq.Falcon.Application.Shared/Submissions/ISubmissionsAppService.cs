using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Submissions
{
	public interface ISubmissionsAppService : IApplicationService 
	{
		Task<PagedResultDto<GetSubmissionForViewDto>> GetAll(GetAllSubmissionsInput input);

		//Task<GetSubmissionForEditOutput> GetSubmissionForEdit(EntityDto<Guid> input);

		Task<GetSubmissionForViewOutput> GetSubmissionForView(EntityDto<Guid> input);

		//Task CreateOrEdit(CreateOrEditSubmissionDto input);

		//Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetSubmissionsToExcel(GetAllSubmissionsForExcelInput input);
		
		//Task<PagedResultDto<SubmissionRecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input);
		
		//Task<PagedResultDto<SubmissionRecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input);
		
		//Task<PagedResultDto<SubmissionUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		//Task<PagedResultDto<SubmissionAppJobLookupTableDto>> GetAllAppJobForLookupTable(GetAllForLookupTableInput input);
	}
}