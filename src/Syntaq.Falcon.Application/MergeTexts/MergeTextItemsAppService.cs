using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.MergeTexts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.MergeTexts
{
	[AbpAuthorize(AppPermissions.Pages_MergeTextItems)]
	public class MergeTextItemsAppService : FalconAppServiceBase, IMergeTextItemsAppService
	{
		private readonly IRepository<MergeTextItem, long> _mergeTextItemRepository;
		private readonly IRepository<MergeTextItemValue, long> _mergeTextItemValueRepository;

		public MergeTextItemsAppService(IRepository<MergeTextItem, long> mergeTextItemRepository, IRepository<MergeTextItemValue, long> mergeTextItemValueRepository) 
		{
			_mergeTextItemRepository = mergeTextItemRepository;
			_mergeTextItemValueRepository = mergeTextItemValueRepository;
		}

		 public async Task<PagedResultDto<GetMergeTextItemForViewDto>> GetAll(GetAllMergeTextItemsInput input)
		 {
			input.Filter = input.Filter?.Trim();

			var filteredMergeTextItems = _mergeTextItemRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim());

			var query = (from o in filteredMergeTextItems					 
						 select new GetMergeTextItemForViewDto() {
							MergeTextItem = new MergeTextItemDto
							{
								Name = o.Name,
								Id = o.Id
							},
							MergeTextItemValueKey = ""/*s1 == null ? "" : s1.Key.ToString()*/
						})
						.WhereIf(!string.IsNullOrWhiteSpace(input.MergeTextItemValueKeyFilter), e => e.MergeTextItemValueKey.ToLower() == input.MergeTextItemValueKeyFilter.ToLower().Trim());

			var totalCount = await query.CountAsync();

			var mergeTextItems = await query
				.OrderBy(input.Sorting ?? "mergeTextItem.id asc")
				.PageBy(input)
				.ToListAsync();

			return new PagedResultDto<GetMergeTextItemForViewDto>(
				totalCount,
				mergeTextItems
			);
		 }

#if STQ_PRODUCTION
		[AbpAuthorize(AppPermissions.Pages_MergeTextItems_Edit)]
		 public async Task<GetMergeTextItemForEditOutput> GetMergeTextItemForEdit(EntityDto<long> input)
		 {
			var mergeTextItem = await _mergeTextItemRepository.FirstOrDefaultAsync(input.Id);
		   
			var output = new GetMergeTextItemForEditOutput {MergeTextItem = ObjectMapper.Map<CreateOrEditMergeTextItemDto>(mergeTextItem)};
			
			return output;
		 }

		[AbpAuthorize(AppPermissions.Pages_MergeTextItems_Edit)]
		public async Task CreateOrEdit(CreateOrEditMergeTextItemDto input)
		 {
			if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItems_Create)]
		 private async Task Create(CreateOrEditMergeTextItemDto input)
		 {
			var mergeTextItem = ObjectMapper.Map<MergeTextItem>(input);

			
			if (AbpSession.TenantId != null)
			{
				mergeTextItem.TenantId = (int?) AbpSession.TenantId;
			}
		

			await _mergeTextItemRepository.InsertAsync(mergeTextItem);
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItems_Edit)]
		 private async Task Update(CreateOrEditMergeTextItemDto input)
		 {
			var mergeTextItem = await _mergeTextItemRepository.FirstOrDefaultAsync((long)input.Id);
			 ObjectMapper.Map(input, mergeTextItem);
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItems_Delete)]
		 public async Task Delete(EntityDto<long> input)
		 {
			await _mergeTextItemRepository.DeleteAsync(input.Id);
		 } 
#endif


	}
}