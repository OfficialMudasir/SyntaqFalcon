using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.MergeTexts.Dtos;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.MergeTexts
{
	[AbpAuthorize(AppPermissions.Pages_MergeTextItemValues)]
	public class MergeTextItemValuesAppService : FalconAppServiceBase, IMergeTextItemValuesAppService
	{
		private readonly IRepository<MergeTextItemValue, long> _mergeTextItemValueRepository;
		private readonly IRepository<MergeTextItem, long> _mergeTextItemRepository;

		public MergeTextItemValuesAppService(IRepository<MergeTextItemValue, long> mergeTextItemValueRepository, IRepository<MergeTextItem, long> mergeTextItemRepository) 
		{
			_mergeTextItemValueRepository = mergeTextItemValueRepository;
			_mergeTextItemRepository = mergeTextItemRepository;
		}

		 public async Task<PagedResultDto<GetMergeTextItemValueForViewDto>> GetAll(GetAllMergeTextItemValuesInput input)
		 {
			input.Filter = input.Filter?.Trim();

			var filteredMergeTextItemValues = _mergeTextItemValueRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Key.Contains(input.Filter) || e.Value.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.KeyFilter),  e => e.Key.ToLower() == input.KeyFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.ValueFilter),  e => e.Value.ToLower() == input.ValueFilter.ToLower().Trim());


			var query = from o in filteredMergeTextItemValues
						//join o1 in _mergeTextItemRepository.GetAll() on o.MergeTextItemId equals o1.Id into j1
						//from s1 in j1.DefaultIfEmpty()
						select new GetMergeTextItemValueForViewDto() {
							MergeTextItemValue = new MergeTextItemValueDto
							{
								Key = o.Key,
								Value = o.Value,
								Id = o.Id
							}
						};

			var totalCount = await query.CountAsync();

			var mergeTextItemValues = await query
				.OrderBy(input.Sorting ?? "mergeTextItemValue.id asc")
				.PageBy(input)
				.ToListAsync();

			return new PagedResultDto<GetMergeTextItemValueForViewDto>(
				totalCount,
				mergeTextItemValues
			);
		 }

#if STQ_PRODUCTION

		[AbpAuthorize(AppPermissions.Pages_MergeTextItemValues_Edit)]
		 public async Task<GetMergeTextItemValueForEditOutput> GetMergeTextItemValueForEdit(EntityDto<long> input)
		 {
			var mergeTextItemValue = await _mergeTextItemValueRepository.FirstOrDefaultAsync(input.Id);
		   
			var output = new GetMergeTextItemValueForEditOutput {MergeTextItemValue = ObjectMapper.Map<CreateOrEditMergeTextItemValueDto>(mergeTextItemValue)};
			
			return output;
		 }

		 public async Task CreateOrEdit(CreateOrEditMergeTextItemValueDto input)
		 {
			if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItemValues_Create)]
		 private async Task Create(CreateOrEditMergeTextItemValueDto input)
		 {
			var mergeTextItemValue = ObjectMapper.Map<MergeTextItemValue>(input);

			
			if (AbpSession.TenantId != null)
			{
				mergeTextItemValue.TenantId = (int?) AbpSession.TenantId;
			}
		

			await _mergeTextItemValueRepository.InsertAsync(mergeTextItemValue);
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItemValues_Edit)]
		 private async Task Update(CreateOrEditMergeTextItemValueDto input)
		 {
			var mergeTextItemValue = await _mergeTextItemValueRepository.FirstOrDefaultAsync((long)input.Id);
			 ObjectMapper.Map(input, mergeTextItemValue);
		 }

		 [AbpAuthorize(AppPermissions.Pages_MergeTextItemValues_Delete)]
		 public async Task Delete(EntityDto<long> input)
		 {
			await _mergeTextItemValueRepository.DeleteAsync(input.Id);
		 }

#endif

	}
}