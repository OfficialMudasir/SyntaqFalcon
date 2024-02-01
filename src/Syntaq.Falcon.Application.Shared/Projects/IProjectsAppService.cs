using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using System.IO;
using Syntaq.Falcon.Records.Dtos;
using GetAllForLookupTableInput = Syntaq.Falcon.Projects.Dtos.GetAllForLookupTableInput;
using System.Collections.Generic;

namespace Syntaq.Falcon.Projects
{
    public interface IProjectsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetProjectForViewDto>> GetAll(GetAllProjectsInput input);
		Task<PagedResultDto<GetProjectForViewDto>> GetForUser(GetAllProjectsInput input);

		Task<PagedResultDto<ShareProjectForViewDto>> GetSharedProjects(GetAllProjectsInput input);

		Task<GetProjectForViewDto> GetProjectForView(Guid id);
		Task<string> StartProject(Guid releaseId, Guid projectId,  string projectname, string projectdescription, Guid? recordId);

		Task<GetProjectForEditOutput> GetProjectForEdit(EntityDto<Guid> input);
		Task CreateOrEdit(CreateOrEditProjectDto input);
		Task Delete(EntityDto<Guid> input);
		Task PublishStep(PublishInput input );

		#if STQ_PRODUCTION
			Task<FileDto> GetProjectsToExcel(GetAllProjectsForExcelInput input);		
			Task<PagedResultDto<ProjectRecordLookupTableDto>> GetAllRecordForLookupTable(GetAllForLookupTableInput input);
		#endif


		//Project Templates part
		Task CreateOrEditProjectTemplate(CreateOrEditProjectTemplateDto input);
		Task DeleteProjectTemplate(EntityDto<Guid> input);
		Task<PagedResultDto<GetProjectTemplatesForViewDto>> GetAllProjectTemplates(GetAllProjectTemplatesInput input);
		Task<PagedResultDto<GetProjectTemplatesForViewDto>> GetAllSharedProjectTemplates(PagedAndSortedResultRequestDto input);
		Task<CreateOrEditProjectTemplateDto> GetProjectTemplatesForEdit(EntityDto<Guid> input);
		Task<GetProjectTemplateForView> GetProjectTemplateForView(Guid id);

        MemoryStream ExportProject(Guid id);
		Task ImportProject(ImportProjectInput input);
        MemoryStream ExportProjectDocument(Guid projectId);
		GetRecordMatterItemForDownload GetDocumentForDownload(Guid input, int version, string format);

		Task<List<GetProjectTemplateForView>> GetVersionHistory(EntityDto<Guid> input);

    }
}