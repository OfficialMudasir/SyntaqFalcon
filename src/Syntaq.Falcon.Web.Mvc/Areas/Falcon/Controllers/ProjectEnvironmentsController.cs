using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectEnvironments;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_ProjectEnvironments)]
    public class ProjectEnvironmentsController : FalconControllerBase
    {
        private readonly IProjectEnvironmentsAppService _projectEnvironmentsAppService;

        public ProjectEnvironmentsController(IProjectEnvironmentsAppService projectEnvironmentsAppService)
        {
            _projectEnvironmentsAppService = projectEnvironmentsAppService;

        }

        public ActionResult Index()
        {
            var model = new ProjectEnvironmentsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectEnvironments_Create, AppPermissions.Pages_ProjectEnvironments_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetProjectEnvironmentForEditOutput getProjectEnvironmentForEditOutput;

            if (id.HasValue)
            {
                getProjectEnvironmentForEditOutput = await _projectEnvironmentsAppService.GetProjectEnvironmentForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getProjectEnvironmentForEditOutput = new GetProjectEnvironmentForEditOutput
                {
                    ProjectEnvironment = new CreateOrEditProjectEnvironmentDto()
                };
            }

            var viewModel = new CreateOrEditProjectEnvironmentModalViewModel()
            {
                ProjectEnvironment = getProjectEnvironmentForEditOutput.ProjectEnvironment,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewProjectEnvironmentModal(Guid id)
        {
            var getProjectEnvironmentForViewDto = await _projectEnvironmentsAppService.GetProjectEnvironmentForView(id);

            var model = new ProjectEnvironmentViewModel()
            {
                ProjectEnvironment = getProjectEnvironmentForViewDto.ProjectEnvironment
            };

            return PartialView("_ViewProjectEnvironmentModal", model);
        }

    }
}