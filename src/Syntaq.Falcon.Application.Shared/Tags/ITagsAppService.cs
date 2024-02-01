using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Tags
{
    public interface ITagsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTagForViewDto>> GetAll(GetAllTagsInput input);

        Task<GetTagForViewDto> GetTagForView(Guid id);

        Task<GetTagForEditOutput> GetTagForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditTagDto input);

        Task Delete(EntityDto<Guid> input);

    }
}