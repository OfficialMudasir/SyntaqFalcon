using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectTenants;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_ProjectTenants)]
    public class ProjectTenantsController : FalconControllerBase
    {
        private readonly IProjectTenantsAppService _projectTenantsAppService;

        public ProjectTenantsController(IProjectTenantsAppService projectTenantsAppService)
        {
            _projectTenantsAppService = projectTenantsAppService;

        }

        public ActionResult Index()
        {
            var model = new ProjectTenantsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectTenants_Create, AppPermissions.Pages_ProjectTenants_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid? projectId)
        {
            GetProjectTenantForEditOutput getProjectTenantForEditOutput;

            if (id.HasValue)
            {
                getProjectTenantForEditOutput = await _projectTenantsAppService.GetProjectTenantForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getProjectTenantForEditOutput = new GetProjectTenantForEditOutput
                {
                    ProjectTenant = new CreateOrEditProjectTenantDto(){ ProjectId = (Guid)projectId, Enabled = true},                    
                };
            }

            var viewModel = new CreateOrEditProjectTenantModalViewModel()
            {
                ProjectTenant = getProjectTenantForEditOutput.ProjectTenant,
                ProjectEnvironmentName = getProjectTenantForEditOutput.ProjectEnvironmentName
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewProjectTenantModal(Guid id)
        {
            var getProjectTenantForViewDto = await _projectTenantsAppService.GetProjectTenantForView(id);

            var model = new ProjectTenantViewModel()
            {
                ProjectTenant = getProjectTenantForViewDto.ProjectTenant,
                ProjectEnvironmentName = getProjectTenantForViewDto.ProjectEnvironmentName
            };

            return PartialView("_ViewProjectTenantModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectTenants_Create, AppPermissions.Pages_ProjectTenants_Edit)]
        public PartialViewResult ProjectEnvironmentLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new ProjectTenantProjectEnvironmentLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_ProjectTenantProjectEnvironmentLookupTableModal", viewModel);
        }

    }
}