using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Records
{
    public interface IRecordMatterItemHistoriesAppService : IApplicationService
    {
        GetRecordMatterItemForDownload GetDocumentForDownload(EntityDto<Guid> id, string format);

        Task<PagedResultDto<GetRecordMatterItemHistoryForViewDto>> GetAll(GetAllRecordMatterItemHistoriesInput input);

        Task<GetRecordMatterItemHistoryForViewDto> GetRecordMatterItemHistoryForView(Guid id);

        Task<GetRecordMatterItemHistoryForEditOutput> GetRecordMatterItemHistoryForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditRecordMatterItemHistoryDto input);

        Task Delete(EntityDto<Guid> input);

        Task<PagedResultDto<RecordMatterItemHistoryRecordMatterItemLookupTableDto>> GetAllRecordMatterItemForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<RecordMatterItemHistoryFormLookupTableDto>> GetAllFormForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<RecordMatterItemHistorySubmissionLookupTableDto>> GetAllSubmissionForLookupTable(GetAllForLookupTableInput input);

    }
}