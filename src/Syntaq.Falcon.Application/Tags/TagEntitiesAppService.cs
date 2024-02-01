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
    [AbpAuthorize(AppPermissions.Pages_TagEntities)]
    public class TagEntitiesAppService : FalconAppServiceBase, ITagEntitiesAppService
    {
        private readonly IRepository<TagEntity, Guid> _tagEntityRepository;
        private readonly IRepository<TagValue, Guid> _lookup_tagValueRepository;

        public TagEntitiesAppService(IRepository<TagEntity, Guid> tagEntityRepository, IRepository<TagValue, Guid> lookup_tagValueRepository)
        {
            _tagEntityRepository = tagEntityRepository;
            _lookup_tagValueRepository = lookup_tagValueRepository;

        }

        public async Task<PagedResultDto<GetTagEntityForViewDto>> GetAll(GetAllTagEntitiesInput input)
        {
            input.Filter = input.Filter?.Trim();

            var entityTypeFilter = input.EntityTypeFilter.HasValue
                        ? (EntityType)input.EntityTypeFilter
                        : default;

            var filteredTagEntities = _tagEntityRepository.GetAll()
                        .Include(e => e.TagValueFk)
                        .Where(e => e.TagValueId != null)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EntityIdFilter.ToString()), e => e.EntityId.ToString() == input.EntityIdFilter.ToString())
                        .WhereIf(input.EntityTypeFilter.HasValue && input.EntityTypeFilter > -1, e => e.EntityType == entityTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TagValueValueFilter), e => e.TagValueFk != null && e.TagValueFk.Value == input.TagValueValueFilter);

            var pagedAndFilteredTagEntities = filteredTagEntities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tagEntities = from o in pagedAndFilteredTagEntities
                              join o1 in _lookup_tagValueRepository.GetAll() on o.TagValueId equals o1.Id into j1
                              from s1 in j1.DefaultIfEmpty()

                              select new
                              {
                                  
                                  o.EntityId,
                                  o.EntityType,
                                  Id = o.Id,
                                  TagValueId = s1.Id,
                                  TagValueValue = s1 == null || s1.Value == null ? "" : s1.Value.ToString()
                              };

            var totalCount = await filteredTagEntities.CountAsync();

            var dbList = await tagEntities.ToListAsync();
            var results = new List<GetTagEntityForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetTagEntityForViewDto()
                {
                    TagEntity = new TagEntityDto
                    {
                        EntityId = o.EntityId,
                        EntityType = o.EntityType,
                        TagValueId = o.TagValueId,
                        Id = o.Id,
                    },

                    TagValueValue = o.TagValueValue
                };

                results.Add(res);
            }

            return new PagedResultDto<GetTagEntityForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetTagEntityForViewDto> GetTagEntityForView(Guid id)
        {
            var tagEntity = await _tagEntityRepository.GetAsync(id);

            var output = new GetTagEntityForViewDto { TagEntity = ObjectMapper.Map<TagEntityDto>(tagEntity) };

            if (output.TagEntity.TagValueId != null)
            {
                var _lookupTagValue = await _lookup_tagValueRepository.FirstOrDefaultAsync((Guid)output.TagEntity.TagValueId);
                output.TagValueValue = _lookupTagValue?.Value?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TagEntities_Edit)]
        public async Task<GetTagEntityForEditOutput> GetTagEntityForEdit(EntityDto<Guid> input)
        {
            var tagEntity = await _tagEntityRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTagEntityForEditOutput { TagEntity = ObjectMapper.Map<CreateOrEditTagEntityDto>(tagEntity) };

            if (output.TagEntity.TagValueId != null)
            {
                var _lookupTagValue = await _lookup_tagValueRepository.FirstOrDefaultAsync((Guid)output.TagEntity.TagValueId);
                output.TagValueValue = _lookupTagValue?.Value?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTagEntityDto input)
        {
            if (input.Id == null || ! _tagEntityRepository.GetAll().Any(e => e.Id == input.Id))
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_TagEntities_Create)]
        protected virtual async Task Create(CreateOrEditTagEntityDto input)
        {
            var tagEntity = ObjectMapper.Map<TagEntity>(input);

            if (AbpSession.TenantId != null)
            {
                tagEntity.TenantId = (int?)AbpSession.TenantId;
            }

            await _tagEntityRepository.InsertAsync(tagEntity);

        }

        [AbpAuthorize(AppPermissions.Pages_TagEntities_Edit)]
        protected virtual async Task Update(CreateOrEditTagEntityDto input)
        {
            var tagEntity = await _tagEntityRepository.FirstOrDefaultAsync((Guid)input.Id);
            if (input.IsSelected)
            {
                ObjectMapper.Map(input, tagEntity);
            }
            else
            {
                // Delete if unchecked
                await _tagEntityRepository.DeleteAsync(new EntityDto<Guid>((Guid)input.Id).Id);
            }
 
        }

        [AbpAuthorize(AppPermissions.Pages_TagEntities_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _tagEntityRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TagEntities)]
        public async Task<PagedResultDto<TagEntityTagValueLookupTableDto>> GetAllTagValueForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_tagValueRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Value != null && e.Value.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var tagValueList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<TagEntityTagValueLookupTableDto>();
            foreach (var tagValue in tagValueList)
            {
                lookupTableDtoList.Add(new TagEntityTagValueLookupTableDto
                {
                    Id = tagValue.Id.ToString(),
                    DisplayName = tagValue.Value?.ToString()
                });
            }

            return new PagedResultDto<TagEntityTagValueLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}