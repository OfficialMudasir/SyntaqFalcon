using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Forms
{
	public interface IFormsAppService : IApplicationService 
	{
		Task<PagedResultDto<FormFolderDto>> GetAll(GetAllFormsInput input);

		Task<List<GetFormForView>> GetVersionHistory(EntityDto<Guid> originalId);

		Task<GetFormForEditOutput> GetFormForEdit(GetFormForViewDto input);
		Task<GetFormForView> GetFormForView(GetFormForViewDto input);
		Task<GetFormForEditOutput> GetFormForAuthor(GetFormForViewDto input);
		Task<GetFormForLoad> GetFormLoadView(GetFormForViewDto input);
		Task<GetFormForLoad> GetFormLoadProjectView(GetFormForViewDto input, Guid? ProjectId);
        Task<GetFormForLoad> GetFormFromReleaseView(Guid releaseId, Guid formId);

        Task CreateOrEdit(CreateOrEditFormDto input);

		Task<MessageOutput> DeleteIndividual(EntityDto<Guid> input);

		Task<MessageOutput> DeleteAll(EntityDto<Guid> input);

		Task<bool> ToggleForm(Guid Id, bool Toggle);

		Task<bool> Run(dynamic input);

		Task<bool> Move(MoveFolderDto moveFolderDto);
        Task<string> GetSchema(EntityDto<Guid> entityDto, string Flag = "Build", int Version = 0);

        Task<bool> SetSchema(string Id, string Schema);
        Task<bool> SetRulesSchema(string Id, string Schema);
        // Task<JArray> GetFormsList();
		Task<string> Save(dynamic input);
        List<FormListDto> GetFormsList(string Flag, string Version);
	}
}