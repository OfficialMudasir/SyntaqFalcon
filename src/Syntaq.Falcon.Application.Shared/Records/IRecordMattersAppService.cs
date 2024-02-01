using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Records
{
	public interface IRecordMattersAppService : IApplicationService 
	{
		PagedResultDto<GetRecordMatterForView> GetAllByRecord(GetAllRecordMattersInput input);

		Task<PagedResultDto<GetRecordMatterForView>> GetAll(GetAllRecordMattersInput input);

		//Task<GetRecordMatterForEditOutput> GetRecordMatterForEdit(EntityDto<Guid> input);
        Task<GetRecordMatterForEditOutput> GetRecordMatterForEdit(Guid id, Guid RecordMatterItemid, Guid? FormId, string accessToken);

        Task CreateOrEdit(CreateOrEditRecordMatterDto input);

		Task<MessageOutput> Delete(EntityDto<Guid> input);

		Task<string> GetRecordMatterJsonData(EntityDto<Guid> input);
		string GetRecordMatterByNameJsonData(string input);

		//Task<PagedResultDto<RecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input);
		
	}
}