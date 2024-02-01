using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Documents
{
    public interface IDocumentsAppService : IApplicationService 
    {
        Task Automate(dynamic JSONObject);

  //      Task<PagedResultDto<GetDocumentForView>> GetAll(GetAllDocumentsInput input);

        //Task<GetDocumentForEditOutput> GetDocumentForEdit(EntityDto input);

        //Task CreateOrEdit(CreateOrEditDocumentDto input);

        //Task Delete(EntityDto input);
    }
}