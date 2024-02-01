using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Records
{
	public interface IRecordMatterItemsAppService : IApplicationService 
	{
		PagedResultDto<GetRecordMatterItemForView> GetAllByRecordMatter(GetAllRecordMatterItemsInput input);

		Task<GetRecordMatterItemForEditOutput> GetRecordMatterItemForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditRecordMatterItemDto input);

		Task Delete(EntityDto<Guid> input);
		
		//Task<PagedResultDto<RecordMatterLookupTableDto>> GetAllRecordMatterForLookupTable(GetAllForLookupTableInput input);

		Task<GetRecordMatterItemForDownload> GetDocumentForDownload(EntityDto<Guid> input, int version, string format, string AccessToken);
	}
}