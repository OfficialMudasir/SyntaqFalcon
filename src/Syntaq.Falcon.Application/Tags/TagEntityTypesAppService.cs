using Syntaq.Falcon.Tags;

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
    [AbpAuthorize(AppPermissions.Pages_TagEntityTypes)]
    public class TagEntityTypesAppService : FalconAppServiceBase, ITagEntityTypesAppService
    {
        private readonly IRepository<TagEntityType, Guid> _tagEntityTypeRepository;
        private readonly IRepository<Tag, Guid> _lookup_tagRepository;

        public TagEntityTypesAppService(IRepository<TagEntityType, Guid> tagEntityTypeRepository, IRepository<Tag, Guid> lookup_tagRepository)
        {
            _tagEntityTypeRepository = tagEntityTypeRepository;
            _lookup_tagRepository = lookup_tagRepository;

        }

        public async Task<PagedResultDto<GetTagEntityTypeForViewDto>> GetAll(GetAllTagEntityTypesInput input)
        {
            input.Filter = input.Filter?.Trim();

            var entityTypeFilter = input.EntityTypeFilter.HasValue
                        ? (EntityType)input.EntityTypeFilter
                        : default;

            var filteredTagEntityTypes = _tagEntityTypeRepository.GetAll()
                        .Include(e => e.TagFk).ThenInclude(tv => tv.Values)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.EntityTypeFilter.HasValue && input.EntityTypeFilter > -1, e => e.EntityType == entityTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TagNameFilter), e => e.TagFk != null && e.TagFk.Name == input.TagNameFilter);

            var pagedAndFilteredTagEntityTypes = filteredTagEntityTypes
                .OrderBy(t => t.TagFk.Name)
                .PageBy(input);

            var tagEntityTypes = from o in pagedAndFilteredTagEntityTypes
                                 join o1 in _lookup_tagRepository.GetAll() on o.TagId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 select new
                                 {
                                     o.EntityType,
                                     Id = o.Id,
                                     TagId = o.TagId,
                                     TagName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                                     Values = s1.Values.OrderBy(v => v.Value).Select(tv => new TagValueDto
                                     {
                                         Id = tv.Id,
                                         TagId = tv.TagId,
                                         Value = tv.Value
                                     })
                                 };

            var totalCount = await filteredTagEntityTypes.CountAsync();

            var dbList = await tagEntityTypes.ToListAsync();
            var results = new List<GetTagEntityTypeForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetTagEntityTypeForViewDto()
                {
                    TagEntityType = new TagEntityTypeDto
                    {
                        TagId = o.TagId,
                        EntityType = o.EntityType,
                        Id = o.Id,
                        Values = o.Values
                    },

                    TagName = o.TagName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetTagEntityTypeForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetTagEntityTypeForViewDto> GetTagEntityTypeForView(Guid id)
        {
            var tagEntityType = await _tagEntityTypeRepository.GetAsync(id);

            var output = new GetTagEntityTypeForViewDto { TagEntityType = ObjectMapper.Map<TagEntityTypeDto>(tagEntityType) };

            if (output.TagEntityType.TagId != null)
            {
                var _lookupTag = await _lookup_tagRepository.FirstOrDefaultAsync((Guid)output.TagEntityType.TagId);
                output.TagName = _lookupTag?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TagEntityTypes_Edit)]
        public async Task<GetTagEntityTypeForEditOutput> GetTagEntityTypeForEdit(EntityDto<Guid> input)
        {
            var tagEntityType = await _tagEntityTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTagEntityTypeForEditOutput { TagEntityType = ObjectMapper.Map<CreateOrEditTagEntityTypeDto>(tagEntityType) };

            if (output.TagEntityType.TagId != null)
            {
                var _lookupTag = await _lookup_tagRepository.FirstOrDefaultAsync((Guid)output.TagEntityType.TagId);
                output.TagName = _lookupTag?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTagEntityTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TagEntityTypes_Create)]
        protected virtual async Task Create(CreateOrEditTagEntityTypeDto input)
        {
            var tagEntityType = ObjectMapper.Map<TagEntityType>(input);

            if (AbpSession.TenantId != null)
            {
                tagEntityType.TenantId = (int?)AbpSession.TenantId;
            }

            await _tagEntityTypeRepository.InsertAsync(tagEntityType);

        }

        [AbpAuthorize(AppPermissions.Pages_TagEntityTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTagEntityTypeDto input)
        {
            var tagEntityType = await _tagEntityTypeRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, tagEntityType);

        }

        [AbpAuthorize(AppPermissions.Pages_TagEntityTypes_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _tagEntityTypeRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TagEntityTypes)]
        public async Task<PagedResultDto<TagEntityTypeTagLookupTableDto>> GetAllTagForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_tagRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var tagList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<TagEntityTypeTagLookupTableDto>();
            foreach (var tag in tagList)
            {
                lookupTableDtoList.Add(new TagEntityTypeTagLookupTableDto
                {
                    Id = tag.Id.ToString(),
                    DisplayName = tag.Name?.ToString()
                });
            }

            return new PagedResultDto<TagEntityTypeTagLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}