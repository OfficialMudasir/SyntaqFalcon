using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.RecordPolicyActions
{
    public interface IRecordPolicyActionsAppService : IApplicationService
    {
        Task<PagedResultDto<GetRecordPolicyActionForViewDto>> GetAll(GetAllRecordPolicyActionsInput input);

        Task<GetRecordPolicyActionForViewDto> GetRecordPolicyActionForView(Guid id);

        Task<GetRecordPolicyActionForEditOutput> GetRecordPolicyActionForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditRecordPolicyActionDto input);

        // Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetRecordPolicyActionsToExcel(GetAllRecordPolicyActionsForExcelInput input);

        Task<PagedResultDto<RecordPolicyActionRecordPolicyLookupTableDto>> GetAllRecordPolicyForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<GetRecordPolicyActionForViewDto>> GetAllByRecordId(GetAllRecordPolicyActionsInput input);

    }
}