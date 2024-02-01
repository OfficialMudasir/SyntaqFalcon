using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Projects;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO.Compression;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Users;
using System.Collections.Generic;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectDeployments;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Projects)]
    public class ProjectsController : FalconControllerBase
    {
        private readonly IProjectsAppService _projectsAppService; 
        private readonly ITagEntityTypesAppService _tagsEntityTypesAppService;
        private readonly IUserAcceptancesAppService _userAcceptancesAppService;

        public ProjectsController(
            IProjectsAppService projectsAppService,
            ITagEntityTypesAppService tagsEntityTypesAppService,
            IUserAcceptancesAppService userAcceptancesAppService
        )
        {
            _projectsAppService = projectsAppService;
            _tagsEntityTypesAppService = tagsEntityTypesAppService;
            _userAcceptancesAppService = userAcceptancesAppService;
        }

        public ActionResult Index(int? statusFilter)
        {
            if (statusFilter != null)
            {
                var model = new ProjectsViewModel
                {
                    StatusFilter = statusFilter,
                    FilterText = ""
                };
                return View(model);
            }
            else
            {
                var model = new ProjectsViewModel
                {
                    FilterText = ""
                };
                return View(model);
            }
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
        public async Task<ActionResult> ArchivedProjects(int? statusFilter)
        {
                var model = new ProjectsViewModel
                {
                    FilterText = ""
                };
                return View(model);
            
        }
        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
        public async Task<ActionResult> ProjectsSharedWithMe(int? statusFilter)
        {
            var model = new ProjectsViewModel
            {
                FilterText = ""
            };
            return View(model);

        }

        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
			public async Task<ActionResult> CreateOrEdit(Guid? id)
			{
				GetProjectForEditOutput getProjectForEditOutput;

				if (id.HasValue){
					getProjectForEditOutput = await _projectsAppService.GetProjectForEdit(new EntityDto<Guid> { Id = (Guid) id });
				}
				else {
					getProjectForEditOutput = new GetProjectForEditOutput{
						Project = new CreateOrEditProjectDto()
					};
				}

				var viewModel = new CreateOrEditProjectViewModel()
				{
					Project = getProjectForEditOutput.Project,
					RecordRecordName = getProjectForEditOutput.RecordRecordName,                
				};

                return PartialView("_CreateOrEditModal", viewModel);
          
			}

        //public async Task<ActionResult> StartProject(Guid id, string projectname)
        //{

        //    var newprojectid = await _projectsAppService.StartProject(id, projectname);
        //    var getProjectForViewDto = await _projectsAppService.GetProjectForView(newprojectid);

        //    var model = new ProjectViewModel()
        //    {
        //        Project = getProjectForViewDto.Project,
        //        RecordRecordName = getProjectForViewDto.RecordRecordName

        //    };

        //    return View("ViewProject", model);

        //}

       
        public async Task<ActionResult> ViewProject(Guid id, Guid? recordMatterId)
        {
            
            var getProjectForViewDto = await _projectsAppService.GetProjectForView(id);
            // set contributor userAcceptance result
            foreach (RecordMatterContributorDto contributor in getProjectForViewDto.Project.Contributors)
            {
                contributor.ContributorAcceptance = _userAcceptancesAppService.GetUserAcceptanceForRecordMatterContributor(contributor.Id, contributor.UserId);
            }
            var model = new ProjectViewModel()
            {
                Project = getProjectForViewDto.Project, 
                RecordRecordName = getProjectForViewDto.RecordRecordName,
                Uploadfiles = getProjectForViewDto.Uploadfiles,
                Deployments = getProjectForViewDto.Deployments,
                Release = getProjectForViewDto.Release

            };
            if (recordMatterId != null)
            {
                model.RecordMatterId = recordMatterId;
            }
            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
        public FileResult ExportDocument(Guid ProjectId)
        {
            MemoryStream returnedProjectDocument = _projectsAppService.ExportProjectDocument(ProjectId);

            byte[] returnedFileByte = returnedProjectDocument.ToArray();
            return File(returnedFileByte.ToArray(), "application/zip", "ProjectDocuments.zip");
        }


        [AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
        public PartialViewResult RecordLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new ProjectRecordLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_ProjectRecordLookupTableModal", viewModel);
        }

		[AbpMvcAuthorize(AppPermissions.Pages_Projects_Create, AppPermissions.Pages_Projects_Edit)]
		//[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> StartProjectModal()
		{

            // Get the Posible Tags for this Project Template
            GetAllTagEntityTypesInput taginput = new GetAllTagEntityTypesInput()
            {
                EntityTypeFilter = (int)EntityType.Project
            };

            PagedResultDto<GetTagEntityTypeForViewDto> tags = new PagedResultDto<GetTagEntityTypeForViewDto>();
            tags = await _tagsEntityTypesAppService.GetAll(taginput);

            //var result = await _projectsAppService.GetForUser(input);
            ProjectsViewModel viewModel = new ProjectsViewModel()
            {
                //Projects = result.Items
                Tags = tags
            };

            return base.PartialView("_StartProjectModal", viewModel);

        }


    }
}