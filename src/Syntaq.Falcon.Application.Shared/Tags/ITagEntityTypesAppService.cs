using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Tags
{
    public interface ITagEntityTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTagEntityTypeForViewDto>> GetAll(GetAllTagEntityTypesInput input);

        Task<GetTagEntityTypeForViewDto> GetTagEntityTypeForView(Guid id);

        Task<GetTagEntityTypeForEditOutput> GetTagEntityTypeForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditTagEntityTypeDto input);

        Task Delete(EntityDto<Guid> input);

        Task<PagedResultDto<TagEntityTypeTagLookupTableDto>> GetAllTagForLookupTable(GetAllForLookupTableInput input);

        //Task<TagProjectBind> checkTagProjectBind(EntityDto<Guid> input);

    }
}