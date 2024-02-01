using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Documents
{
	public interface ITemplatesAppService : IApplicationService 
	{
		Task<PagedResultDto<TemplateFoldersDto>> GetAll(GetAllTemplatesInput input);

		Task<GetTemplateForEditOutput> GetTemplateForEdit(EntityDto<Guid> input);
		Task<GetTemplateForEditOutput> GetTemplateForUserAcceptanceViewers(EntityDto<Guid> input);

		Task<GetTemplateForEditOutput> GetTemplateForDownload(Guid OriginalId, string version);

		//Task UpdateDocumentTemplate(Guid Id);

		Task<List<GetTemplateForView>> GetVersionHistory(Guid originalId);

		Task CreateOrEdit(CreateOrEditTemplateDto input);

		Task<MessageOutput> DeleteIndividual(EntityDto<Guid> input);

		Task<MessageOutput> DeleteAll(EntityDto<Guid> input);

		Task SetCurrent(TemplateVersionDto templateVersionDto);

		Task<bool> Move(MoveFolderDto moveFolderDto);
	}
}