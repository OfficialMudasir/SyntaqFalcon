using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
 

namespace Syntaq.Falcon.Records
{
    public interface IRecordMatterContributorsAppService : IApplicationService 
    {

        //Task<PagedResultDto<GetRecordMatterContributorForViewDto>> GetAll(GetAllRecordMatterContributorsInput input);

        Task<GetRecordMatterContributorForViewDto> GetRecordMatterContributorForView(Guid id);

		Task<GetRecordMatterContributorForEditOutput> GetRecordMatterContributorForEdit(EntityDto<Guid> Id, EntityDto<Guid> recordmatterId, EntityDto<Guid> formId);

		Task CreateOrEdit(CreateOrEditRecordMatterContributorDto input);

		Task SendInvite(CreateOrEditRecordMatterContributorDto input);

		Task Apply(ApplyInput input);

		Task UpdateComments(UpdateCommentsInput input);

		Task Delete(EntityDto<Guid> input);

		Task Enable(EntityDto<Guid> input);
		Task Disable(EntityDto<Guid> input);

		string GetAccessURL(CreateOrEditRecordMatterContributorDto input);

#if STQ_PRODUCTION
		Task<PagedResultDto<RecordMatterContributorUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
#endif

		//Task<PagedResultDto<RecordMatterContributorFormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input);
		//Task<FileDto> GetRecordMatterContributorsToExcel(GetAllRecordMatterContributorsForExcelInput input);
		//Task<PagedResultDto<RecordMatterContributorRecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input);

	}
}