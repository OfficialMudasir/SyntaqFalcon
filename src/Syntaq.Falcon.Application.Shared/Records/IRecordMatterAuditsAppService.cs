using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Records
{
    public interface IRecordMatterAuditsAppService : IApplicationService
    {
        Task<PagedResultDto<GetRecordMatterAuditForViewDto>> GetAll(GetAllRecordMatterAuditsInput input);

        Task<GetRecordMatterAuditForViewDto> GetRecordMatterAuditForView(Guid id);

        Task<GetRecordMatterAuditForEditOutput> GetRecordMatterAuditForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditRecordMatterAuditDto input);

        Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetRecordMatterAuditsToExcel(GetAllRecordMatterAuditsForExcelInput input);

        Task<PagedResultDto<RecordMatterAuditUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<RecordMatterAuditRecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input);

    }
}