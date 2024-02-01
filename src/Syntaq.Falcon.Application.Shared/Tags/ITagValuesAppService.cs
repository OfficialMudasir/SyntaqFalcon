using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Dto;
using System.Collections.Generic;
using Syntaq.Falcon.Tags;


using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;

using Abp.Domain.Repositories;

using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Abp.UI;
using Abp.Linq;

namespace Syntaq.Falcon.Tags
{
    public interface ITagValuesAppService : IApplicationService
    {


        Task<PagedResultDto<GetTagValueForViewDto>> GetAll(GetAllTagValuesInput input);

        Task<GetTagValueForViewDto> GetTagValueForView(Guid id);

        Task<GetTagValueForEditOutput> GetTagValueForEdit(EntityDto<Guid> input);

        Task<TagHasValue> GetTagValueByTagId(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditTagValueDto input);

        Task Delete(EntityDto<Guid> input);

        Task<PagedResultDto<TagValueTagLookupTableDto>> GetAllTagForLookupTable(GetAllForLookupTableInput input);

    }
}