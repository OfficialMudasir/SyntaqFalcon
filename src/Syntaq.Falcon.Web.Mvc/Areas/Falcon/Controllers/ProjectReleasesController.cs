using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectReleases;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_ProjectReleases)]
    public class ProjectReleasesController : FalconControllerBase
    {
        private readonly IProjectReleasesAppService _projectReleasesAppService;
        private readonly IProjectsAppService _projectsAppService;
        public ProjectReleasesController(IProjectReleasesAppService projectReleasesAppService, IProjectsAppService projectsAppService)
        {
            _projectReleasesAppService = projectReleasesAppService;
            _projectsAppService = projectsAppService;
        }

        public ActionResult Index()
        {
            var model = new ProjectReleasesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectReleases_Create, AppPermissions.Pages_ProjectReleases_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid projectTemplateId, Guid projectId, Guid releaseIdToClone, bool isViewMode)
        {
            GetProjectReleaseForEditOutput getProjectReleaseForEditOutput;

            if (id.HasValue)
            {
                getProjectReleaseForEditOutput = await _projectReleasesAppService.GetProjectReleaseForEdit(new EntityDto<Guid> { Id = (Guid)id });
                getProjectReleaseForEditOutput.ProjectRelease.ReleaseIdToClone = releaseIdToClone;

                if (releaseIdToClone != Guid.Empty || ! getProjectReleaseForEditOutput.HasDeployments)
                {
                    isViewMode = false;
                }                
            }
            else
            {

                // Get the version numbers etc
                GetProjectReleaseVersionForEditOutput prVer = await _projectReleasesAppService.GetProjectReleaseVersionForEdit(new EntityDto<Guid> { Id = (Guid)projectId });
                
                // Get Version form the database based on Id and assign this version when create new release
                var GetProjectVersion = await _projectsAppService.GetProjectTemplatesForEdit(new EntityDto<Guid> { Id = (Guid)projectTemplateId });
                getProjectReleaseForEditOutput = new GetProjectReleaseForEditOutput
                {
                    ProjectRelease = new CreateOrEditProjectReleaseDto { 
                        ProjectId = projectId,
                        ProjectTemplateId = projectTemplateId,
                        //VersionMajor = prVer.VersionMajor,
                        VersionMajor = GetProjectVersion.Version,
                        VersionMinor = prVer.VersionMinor,
                        VersionRevision = prVer.VersionRevision
                    } 
                };

            }

            var viewModel = new CreateOrEditProjectReleaseModalViewModel()
            {
                ProjectRelease = getProjectReleaseForEditOutput.ProjectRelease,
                ProjectEnvironmentName = getProjectReleaseForEditOutput.ProjectEnvironmentName,
                IsViewMode = isViewMode
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewProjectReleaseModal(Guid id)
        {
            var getProjectReleaseForViewDto = await _projectReleasesAppService.GetProjectReleaseForView(id);

            var model = new ProjectReleaseViewModel()
            {
                ProjectRelease = getProjectReleaseForViewDto.ProjectRelease
                ,
                ProjectEnvironmentName = getProjectReleaseForViewDto.ProjectEnvironmentName

            };

            return PartialView("_ViewProjectReleaseModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_ProjectReleases_Create, AppPermissions.Pages_ProjectReleases_Edit)]
        public PartialViewResult ProjectEnvironmentLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new ProjectReleaseProjectEnvironmentLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_ProjectReleaseProjectEnvironmentLookupTableModal", viewModel);
        }

    }
}