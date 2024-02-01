using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectDeployments;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_ProjectDeployments)]
    public class ProjectDeploymentsController : FalconControllerBase
    {
        private readonly IProjectDeploymentsAppService _projectDeploymentsAppService;

        public ProjectDeploymentsController(IProjectDeploymentsAppService projectDeploymentsAppService)
        {
            _projectDeploymentsAppService = projectDeploymentsAppService;

        }

        public ActionResult Index()
        {
            var model = new ProjectDeploymentsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectDeployments_Create, AppPermissions.Pages_ProjectDeployments_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetProjectDeploymentForEditOutput getProjectDeploymentForEditOutput;

            if (id.HasValue)
            {
                getProjectDeploymentForEditOutput = await _projectDeploymentsAppService.GetProjectDeploymentForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getProjectDeploymentForEditOutput = new GetProjectDeploymentForEditOutput
                {
                    ProjectDeployment = new CreateOrEditProjectDeploymentDto()
                };
            }

            var viewModel = new CreateOrEditProjectDeploymentModalViewModel()
            {
                ProjectDeployment = getProjectDeploymentForEditOutput.ProjectDeployment,
                ProjectReleaseName = getProjectDeploymentForEditOutput.ProjectReleaseName,
                ProjectReleaseNotes = getProjectDeploymentForEditOutput.ProjectReleaseNotes
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewProjectDeploymentModal(Guid id)
        {
            var getProjectDeploymentForViewDto = await _projectDeploymentsAppService.GetProjectDeploymentForView(id);

            var model = new ProjectDeploymentViewModel()
            {
                ProjectDeployment = getProjectDeploymentForViewDto.ProjectDeployment
                ,
                ProjectReleaseName = getProjectDeploymentForViewDto.ProjectReleaseName

            };

            return PartialView("_ViewProjectDeploymentModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectDeployments_Create, AppPermissions.Pages_ProjectDeployments_Edit)]
        public PartialViewResult ProjectReleaseLookupTableModal(Guid? id,Guid? projectId, Guid? projectTemplateId, string displayName)
        {
            var viewModel = new ProjectDeploymentProjectReleaseLookupTableViewModel()
            {
                ProjectId = projectId.ToString(),
                ProjectTemplateId = projectTemplateId.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_ProjectDeploymentProjectReleaseLookupTableModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectDeployments)]
        public PartialViewResult ProjectDeploymentTableModal(Guid? id, Guid? environmentId)
        {
            var viewModel = new ProjectDeploymentProjectReleaseLookupTableViewModel()
            {
                ProjectTemplateId = id.ToString(),
                ProjectEnvironmentId = environmentId.ToString(),
                DisplayName = "",
                FilterText = ""
            };

            return PartialView("_ProjectDeploymentTableModal", viewModel);
        }

    }
}