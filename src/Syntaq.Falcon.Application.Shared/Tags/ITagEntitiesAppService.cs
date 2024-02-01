using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Tags
{
    public interface ITagEntitiesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTagEntityForViewDto>> GetAll(GetAllTagEntitiesInput input);

        Task<GetTagEntityForViewDto> GetTagEntityForView(Guid id);

        Task<GetTagEntityForEditOutput> GetTagEntityForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditTagEntityDto input);

        Task Delete(EntityDto<Guid> input);

        Task<PagedResultDto<TagEntityTagValueLookupTableDto>> GetAllTagValueForLookupTable(GetAllForLookupTableInput input);

    }
}