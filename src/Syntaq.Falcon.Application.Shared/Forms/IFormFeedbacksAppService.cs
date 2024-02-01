using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;


namespace Syntaq.Falcon.Forms
{
    public interface IFormFeedbacksAppService : IApplicationService 
    {
        Task<PagedResultDto<GetFormFeedbackForViewDto>> GetAll(GetAllFormFeedbacksInput input);

        Task<GetFormFeedbackForViewDto> GetFormFeedbackForView(Guid id);

		Task<GetFormFeedbackForEditOutput> GetFormFeedbackForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditFormFeedbackDto input);

		Task Delete(EntityDto<Guid> input);

		Task<FileDto> GetFormFeedbacksToExcel(GetAllFormFeedbacksForExcelInput input);

		
		Task<PagedResultDto<FormFeedbackFormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input);

		Task<bool> SendFeedback(CreateOrEditFormFeedbackDto input);


	}
}