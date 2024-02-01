using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Forms
{
    public interface IFormsFoldersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetFormsFolderForView>> GetAll(GetAllFormsFoldersInput input);

		Task<GetFormsFolderForEditOutput> GetFormsFolderForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditFormsFolderDto input);

		Task Delete(EntityDto<Guid> input);

		
    }
}