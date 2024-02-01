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
using Abp.Linq;
using Syntaq.Falcon.ProjectTemplates.Dtos;

namespace Syntaq.Falcon.Tags
{
    [AbpAuthorize(AppPermissions.Pages_TagValues)]
    public class TagValuesAppService : FalconAppServiceBase, ITagValuesAppService
    {
        private readonly IRepository<TagValue, Guid> _tagValueRepository;
        
        private readonly IRepository<Tag, Guid> _lookup_tagRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        

        public TagValuesAppService(IRepository<TagValue, Guid> tagValueRepository, IRepository<Tag, Guid> lookup_tagRepository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _tagValueRepository = tagValueRepository;
            _lookup_tagRepository = lookup_tagRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;

        }

        // Refactor
        public async Task<TagHasValue> GetTagValueByTagId(EntityDto<Guid> input)
        {
            // input.ID is the id from [SfaTags] table
            var tagValue = await _tagValueRepository.FirstOrDefaultAsync(tv => tv.TagId == input.Id);
            var output = new TagHasValue();
            if (tagValue != null)
            {
                output.ifTagHasValue = true;
            }
            else
            {
                output.ifTagHasValue = false;
            }
            return output;
        }

        public async Task<PagedResultDto<GetTagValueForViewDto>> GetAll(GetAllTagValuesInput input)
        {

            input.Filter = input.Filter?.Trim();

            var filteredTagValues = _tagValueRepository.GetAll()
                        .Include(e => e.TagFk)
                        .WhereIf(input.Id != null, e => e.TagId == input.Id)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Value.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ValueFilter), e => e.Value == input.ValueFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TagNameFilter), e => e.TagFk != null && e.TagFk.Name == input.TagNameFilter);

            var pagedAndFilteredTagValues = filteredTagValues
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tagValues = from o in pagedAndFilteredTagValues
                            join o1 in _lookup_tagRepository.GetAll() on o.TagId equals o1.Id into j1
                            from s1 in j1.DefaultIfEmpty()

                            select new
                            {

                                o.Value,
                                Id = o.Id,
                                TagName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                            };

            var totalCount = await filteredTagValues.CountAsync();

            var dbList = await tagValues.ToListAsync();
            var results = new List<GetTagValueForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetTagValueForViewDto()
                {
                    TagValue = new TagValueDto
                    {

                        Value = o.Value,
                        Id = o.Id,
                    },
                    TagName = o.TagName
                };

                results.Add(res);
            }

            return new PagedResultDto<GetTagValueForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetTagValueForViewDto> GetTagValueForView(Guid id)
        {
            var tagValue = await _tagValueRepository.GetAsync(id);

            var output = new GetTagValueForViewDto { TagValue = ObjectMapper.Map<TagValueDto>(tagValue) };

            if (output.TagValue.TagId != null)
            {
                var _lookupTag = await _lookup_tagRepository.FirstOrDefaultAsync((Guid)output.TagValue.TagId);
                output.TagName = _lookupTag?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TagValues_Edit)]
        public async Task<GetTagValueForEditOutput> GetTagValueForEdit(EntityDto<Guid> input)
        {
            // input.ID is the id from [SfaTags] table
            var tagValue = await _tagValueRepository.FirstOrDefaultAsync(tv=>tv.TagId == input.Id);

            var output = new GetTagValueForEditOutput { TagValue = ObjectMapper.Map<CreateOrEditTagValueDto>(tagValue) };

            if (output.TagValue.TagId != null)
            {
                var _lookupTag = await _lookup_tagRepository.FirstOrDefaultAsync((Guid)output.TagValue.TagId);
                output.TagName = _lookupTag?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTagValueDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TagValues_Create)]
        protected virtual async Task Create(CreateOrEditTagValueDto input)
        {
            var tagValue = ObjectMapper.Map<TagValue>(input);

            if (AbpSession.TenantId != null)
            {
                tagValue.TenantId = (int?)AbpSession.TenantId;
            }

            await _tagValueRepository.InsertAsync(tagValue);

        }

        [AbpAuthorize(AppPermissions.Pages_TagValues_Edit)]
        protected virtual async Task Update(CreateOrEditTagValueDto input)
        {
            var tagValue = await _tagValueRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, tagValue);

        }

        [AbpAuthorize(AppPermissions.Pages_TagValues_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _tagValueRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_TagValues)]
        public async Task<PagedResultDto<TagValueTagLookupTableDto>> GetAllTagForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_tagRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var tagList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<TagValueTagLookupTableDto>();
            foreach (var tag in tagList)
            {
                lookupTableDtoList.Add(new TagValueTagLookupTableDto
                {
                    Id = tag.Id.ToString(),
                    DisplayName = tag.Name?.ToString()
                });
            }

            return new PagedResultDto<TagValueTagLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}