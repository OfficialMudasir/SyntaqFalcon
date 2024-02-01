using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Records
{
    public interface IRecordsAppService : IApplicationService 
    {
        //Task<List<ResultsDto>> GetBreadcrumbs(string Id);

        Task<PagedResultDto<ResultsWithMattersDto>> GetAll(GetAllRecordsInput input);

        Task<string> GetRecordJSONData(Guid id);

        //RecordDto GetRecord(string Id, string Name, long? UserId, long? OrgId);

        Task<GetRecordForEditOutput> GetRecordForEdit(EntityDto<Guid> input);

        Task CreateOrEdit(CreateOrEditRecordDto input);

        Task<MessageOutput> Delete(EntityDto<Guid> input);
        
        Task<bool> Move(MoveFolderDto moveFolderDto);

        Task<MessageOutput> Archive(EntityDto<Guid> input);
        Task<MessageOutput> UnArchive(EntityDto<Guid> input);

    }
}