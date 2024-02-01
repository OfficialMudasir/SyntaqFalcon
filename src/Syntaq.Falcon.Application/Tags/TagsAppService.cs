using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Tags
{
    [AbpAuthorize(AppPermissions.Pages_Tags)]
    public class TagsAppService : FalconAppServiceBase, ITagsAppService
    {
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<TagValue, Guid> _tagValueRepository;
        private readonly IRepository<TagEntityType, Guid> _tagEntityTypeRepository;

        public TagsAppService(IRepository<Tag, Guid> tagRepository, IRepository<TagValue, Guid> tagValueRepository, IRepository<TagEntityType, Guid> tagEntityTypeRepository)
        {
            _tagRepository = tagRepository;
            _tagValueRepository = tagValueRepository;
            _tagEntityTypeRepository = tagEntityTypeRepository;

        }

        public async Task<PagedResultDto<GetTagForViewDto>> GetAll(GetAllTagsInput input)
        {
            input.Filter = input.Filter?.Trim();

            var filteredTags = _tagRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);

            var pagedAndFilteredTags = filteredTags
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tags = from o in pagedAndFilteredTags
                       select new
                       {

                           o.Name,
                           Id = o.Id
                       };

            var totalCount = await filteredTags.CountAsync();

            var dbList = await tags.ToListAsync();
            var results = new List<GetTagForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetTagForViewDto()
                {
                    Tag = new TagDto
                    {

                        Name = o.Name,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetTagForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetTagForViewDto> GetTagForView(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            var output = new GetTagForViewDto { Tag = ObjectMapper.Map<TagDto>(tag) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Tags_Edit)]
        public async Task<GetTagForEditOutput> GetTagForEdit(EntityDto<Guid> input)
        {
            var tag = await _tagRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTagForEditOutput { Tag = ObjectMapper.Map<CreateOrEditTagDto>(tag) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTagDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tags_Create)]
        protected virtual async Task Create(CreateOrEditTagDto input)
        {
            var tag = ObjectMapper.Map<Tag>(input);

            if (AbpSession.TenantId != null)
            {
                tag.TenantId = (int?)AbpSession.TenantId;
            }

            await _tagRepository.InsertAsync(tag);

            CreateOrEditTagEntityTypeDto inputData = new CreateOrEditTagEntityTypeDto
            {
                EntityType = EntityType.Project,
                TagId = tag.Id
            };
            await CreateTagEntity(inputData);
        }

        // Create Tag Entity
        protected virtual async Task CreateTagEntity(CreateOrEditTagEntityTypeDto input)
        {
            var tagEntityType = ObjectMapper.Map<TagEntityType>(input);

            if (AbpSession.TenantId != null)
            {
                tagEntityType.TenantId = (int?)AbpSession.TenantId;
            }

            await _tagEntityTypeRepository.InsertAsync(tagEntityType);

        }

        [AbpAuthorize(AppPermissions.Pages_Tags_Edit)]
        protected virtual async Task Update(CreateOrEditTagDto input)
        {
            var tag = await _tagRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, tag);

        }

        [AbpAuthorize(AppPermissions.Pages_Tags_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var tagValue = await _tagValueRepository.FirstOrDefaultAsync(tv => tv.TagId == input.Id);
            if(tagValue != null)
            {
                throw new UserFriendlyException(L("TagHasValue"));
            }
            var tagEntityType = await _tagEntityTypeRepository.FirstOrDefaultAsync(tv => tv.TagId == input.Id);
            if (tagEntityType != null)
            {
                await _tagEntityTypeRepository.DeleteAsync(tagEntityType.Id);
            }
            await _tagRepository.DeleteAsync(input.Id);
        }

    }
}