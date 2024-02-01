using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.RecordPolicies
{
    public interface IRecordPoliciesAppService : IApplicationService
    {
        Task<PagedResultDto<GetRecordPolicyForViewDto>> GetAll(GetAllRecordPoliciesInput input);

        Task<GetRecordPolicyForViewDto> GetRecordPolicyForView(Guid id);

        Task<GetRecordPolicyForEditOutput> GetRecordPolicyForEdit(EntityDto<Guid> input);
        Task<GetRecordPolicyForEditOutput> GetRecordPolicyForCreate();



        Task<Guid> CreateOrEdit(CreateOrEditRecordPolicyDto input);

        //Task Delete(EntityDto<Guid> input);

        Task<FileDto> GetRecordPoliciesToExcel(GetAllRecordPoliciesForExcelInput input);

    }
}