using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Folders
{
	public interface IFoldersAppService : IApplicationService 
	{
		//Task<PagedResultDto<GetFolderForView>> GetAll(GetAllFoldersInput input);

		Task<List<FolderDto>> GetBreadcrumbs(string Id, string Type);

		Task<GetFolderForEditOutput> GetFolderForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditFolderDto input);

		Task<MessageOutput> Delete(EntityDto<Guid> input);

		
	}
}