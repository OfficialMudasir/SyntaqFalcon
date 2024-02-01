using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.MergeTexts.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.MergeTexts
{
	public interface IMergeTextItemsAppService : IApplicationService 
	{
		Task<PagedResultDto<GetMergeTextItemForViewDto>> GetAll(GetAllMergeTextItemsInput input);

#if STQ_PRODUCTION
		Task<GetMergeTextItemForEditOutput> GetMergeTextItemForEdit(EntityDto<long> input);
		Task CreateOrEdit(CreateOrEditMergeTextItemDto input);
		Task Delete(EntityDto<long> input);
#endif
		
		//Task<PagedResultDto<MergeTextItemMergeTextItemValueLookupTableDto>> GetAllMergeTextItemValueForLookupTable(GetAllForLookupTableInput input);
		
	}
}