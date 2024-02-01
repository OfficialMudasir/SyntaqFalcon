using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.MergeTexts.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.MergeTexts
{
	[AbpAuthorize(AppPermissions.Pages_MergeTexts)]
	public class MergeTextsAppService : FalconAppServiceBase, IMergeTextsAppService
	{
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<MergeText, long> _mergeTextRepository;
		private readonly IRepository<MergeTextItem, long> _mergeTextItemRepository;
		private readonly IRepository<MergeTextItemValue, long> _mergeTextItemValueRepository;

		public MergeTextsAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<MergeText, long> mergeTextRepository, IRepository<MergeTextItem, long> mergeTextItemRepository, IRepository<MergeTextItemValue, long> mergeTextItemValueRepository) 
		{
			_unitOfWorkManager = unitOfWorkManager;
			_mergeTextRepository = mergeTextRepository;
			_mergeTextItemRepository = mergeTextItemRepository;
			_mergeTextItemValueRepository = mergeTextItemValueRepository;
		}

		//public async Task<PagedResultDto<GetMergeTextForViewDto>> GetAll(GetAllMergeTextsInput input)
		//{
		//	var filteredMergeTexts = _mergeTextRepository.GetAll()
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EntityType.Contains(input.Filter) || e.EntityKey.Contains(input.Filter))
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter),  e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim())
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.EntityKeyFilter),  e => e.EntityKey.ToLower() == input.EntityKeyFilter.ToLower().Trim());

		//	var query = (from o in filteredMergeTexts 
		//					select new GetMergeTextForViewDto() {
		//					MergeText = new MergeTextDto
		//					{
		//						EntityType = o.EntityType,
		//						EntityKey = o.EntityKey,
		//						Id = o.Id
		//					},
		//					MergeTextItemName = ""/*s1 == null ? "" : s1.Name.ToString()*/
		//				})
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.MergeTextItemNameFilter), e => e.MergeTextItemName.ToLower() == input.MergeTextItemNameFilter.ToLower().Trim());

		//	var totalCount = await query.CountAsync();

		//	var mergeTexts = await query
		//		.OrderBy(input.Sorting ?? "mergeText.id asc")
		//		.PageBy(input)
		//		.ToListAsync();

		//	return new PagedResultDto<GetMergeTextForViewDto>(
		//		totalCount,
		//		mergeTexts
		//	);
		//}

 
		public async Task<GetMergeTextForViewDto> GetMergeTextForView(string MergeTextEntityType, string MergeTextEntityKey)
		{
			MergeText mergeText =  _mergeTextRepository.GetAll().Where(i => i.EntityType == MergeTextEntityType && i.EntityKey == MergeTextEntityKey).FirstOrDefault();
			GetMergeTextForViewDto output = new GetMergeTextForViewDto { MergeText = ObjectMapper.Map<MergeTextDto>(mergeText) };
			return output;
		}

		//public async Task<GetMergeTextForViewDto> GetMergeTextForView(long id)
		//{
		//	MergeText mergeText = await _mergeTextRepository.GetAsync(id);

		//	GetMergeTextForViewDto output = new GetMergeTextForViewDto { MergeText = ObjectMapper.Map<MergeTextDto>(mergeText) };

		//	if (output.MergeText.MergeTextItemId != null)
		//	{
		//		var _lookupMergeTextItem = await _mergeTextItemRepository.FirstOrDefaultAsync((long)output.MergeText.MergeTextItemId);
		//		output.MergeTextItemName = _lookupMergeTextItem.Name.ToString();
		//	}
			
		//	return output;
		//}
		 
		 //[AbpAuthorize(AppPermissions.Pages_MergeTexts_Edit)]
		 //public async Task<GetMergeTextForEditOutput> GetMergeTextForEdit(EntityDto<long> input)
		 //{
			//MergeText mergeText = await _mergeTextRepository.FirstOrDefaultAsync(input.Id);

			//GetMergeTextForEditOutput output = new GetMergeTextForEditOutput {MergeText = ObjectMapper.Map<CreateOrEditMergeTextDto>(mergeText)};

			//if (output.MergeText.MergeTextItemId != null)
			//{
			//	var _lookupMergeTextItem = await _mergeTextItemRepository.FirstOrDefaultAsync((long)output.MergeText.MergeTextItemId);
			//	output.MergeTextItemName = _lookupMergeTextItem.Name.ToString();
			//}
			
			//return output;
		 //}

		public async Task<MergeTextItemWithValues> CreateOrEditMergeTextItem(CreateOrEditMergeTextItemDto input)
		{
			MergeTextItemWithValues _mergeTextItemWithValues = new MergeTextItemWithValues();
			if (input.Id == null){
				_mergeTextItemWithValues = await CreateMergeTextItem(input);
			}
			else{
				_mergeTextItemWithValues = await UpdateMergeTextItem(input);
			}
			return _mergeTextItemWithValues;
		}

		[AbpAuthorize(AppPermissions.Pages_MergeTexts_Create)]
		private async Task<MergeTextItemWithValues> CreateMergeTextItem(CreateOrEditMergeTextItemDto input)
		{
			MergeTextItemWithValues _mergeTextItemWithValues = new MergeTextItemWithValues();

			MergeTextItem _mergeTextItem = ObjectMapper.Map<MergeTextItem>(input);
			await _mergeTextItemRepository.InsertAsync(_mergeTextItem);
			_mergeTextItemWithValues.Id = _mergeTextItem.Id;
			_mergeTextItemWithValues.Name = _mergeTextItem.Name;

			input.MTextList?.ForEach(async i =>
			{
				MergeTextItemValue _mergeTextItemValue = i.Id != null ? _mergeTextItemValueRepository.FirstOrDefault((long)i.Id) : null;
				if (_mergeTextItemValue == null)
				{
					CreateOrEditMergeTextItemValueDto _mergeTextItemValueDto = new CreateOrEditMergeTextItemValueDto()
					{
						Key = i.Key,
						Value = i.Value,
						MergeTextItemId = _mergeTextItem.Id
					};
					await CreateMergeTextItemValue(_mergeTextItemValueDto);
					_mergeTextItemWithValues.ItemValues.Add(ObjectMapper.Map<MergeTextItemValueDto>(_mergeTextItemValueDto));
				}
			});
			return _mergeTextItemWithValues;
		}

		[AbpAuthorize(AppPermissions.Pages_MergeTexts_Create)]
		public async Task<MergeText> CreateMergeText(CreateOrEditMergeTextDto input)
		{
			MergeText _mergeText = new MergeText()
			{
				TenantId = AbpSession.TenantId ?? null,
				EntityType = input.MergeTextEntityType,
				EntityKey = input.MergeTextEntityKey
			};
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				await _mergeTextRepository.InsertAsync(_mergeText);
				unitOfWork.Complete();
			}
			return _mergeText;
		}

		private async Task CreateMergeTextItemValue(CreateOrEditMergeTextItemValueDto input)
		{
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				var mergeTextItemValue = ObjectMapper.Map<MergeTextItemValue>(input);
				if (AbpSession.TenantId != null)
				{
					mergeTextItemValue.TenantId = (int?)AbpSession.TenantId;
				}
				await _mergeTextItemValueRepository.InsertAsync(mergeTextItemValue);
				unitOfWork.Complete();
			}
		}

		[AbpAuthorize(AppPermissions.Pages_MergeTexts_Edit)]
		private async Task<MergeTextItemWithValues> UpdateMergeTextItem(CreateOrEditMergeTextItemDto input)
		{
			MergeTextItemWithValues _mergeTextItemWithValues = new MergeTextItemWithValues();
			//MergeText _mergeText = _mergeTextRepository.FirstOrDefault((long)input.Id);
			//MergeText mergeText = await _mergeTextRepository.FirstOrDefaultAsync((long)input.Id);
			//ObjectMapper.Map(input, mergeText);
			return _mergeTextItemWithValues; //= ObjectMapper.Map<MergeTextItemWithValues>(_mergeTextItemValueDto)
		}

		[AbpAuthorize(AppPermissions.Pages_MergeTexts_Delete)]
		public async Task Delete(EntityDto<long> input)
		{
			await _mergeTextRepository.DeleteAsync(input.Id);
		} 

		[AbpAuthorize(AppPermissions.Pages_MergeTexts)]
		public async Task<PagedResultDto<MergeTextMergeTextItemLookupTableDto>> GetAllMergeTextItemForLookupTable(GetAllForLookupTableInput input)
		{
			var query = _mergeTextItemRepository.GetAll().WhereIf(
				!string.IsNullOrWhiteSpace(input.Filter),
				e=> e.Name.ToString().Contains(input.Filter)
			);

			var totalCount = await query.CountAsync();

			var mergeTextItemList = await query
				.PageBy(input)
				.ToListAsync();

			var lookupTableDtoList = new List<MergeTextMergeTextItemLookupTableDto>();
			foreach(var mergeTextItem in mergeTextItemList){
				lookupTableDtoList.Add(new MergeTextMergeTextItemLookupTableDto
				{
					Id = mergeTextItem.Id,
					DisplayName = mergeTextItem.Name?.ToString()
				});
			}

			return new PagedResultDto<MergeTextMergeTextItemLookupTableDto>(
				totalCount,
				lookupTableDtoList
			);
		}
 

	}
}