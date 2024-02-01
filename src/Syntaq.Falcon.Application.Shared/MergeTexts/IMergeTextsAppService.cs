using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.MergeTexts.Dtos;

namespace Syntaq.Falcon.MergeTexts
{
	public interface IMergeTextsAppService : IApplicationService 
	{
		//Task<PagedResultDto<GetMergeTextForViewDto>> GetAll(GetAllMergeTextsInput input);

 
		Task<GetMergeTextForViewDto> GetMergeTextForView(string MergeTextEntityType, string MergeTextEntityKey);
		Task<MergeTextItemWithValues> CreateOrEditMergeTextItem(CreateOrEditMergeTextItemDto input);
		Task Delete(EntityDto<long> input);
		Task<PagedResultDto<MergeTextMergeTextItemLookupTableDto>> GetAllMergeTextItemForLookupTable(GetAllForLookupTableInput input);		
 

		//Task<GetMergeTextForViewDto> GetMergeTextForView(long id);
		//Task<GetMergeTextForEditOutput> GetMergeTextForEdit(EntityDto<long> input);

	}
}