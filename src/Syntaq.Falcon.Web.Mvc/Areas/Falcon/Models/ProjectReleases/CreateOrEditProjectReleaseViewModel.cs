using Syntaq.Falcon.Projects.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectReleases
{
    public class CreateOrEditProjectReleaseModalViewModel
    {
        public CreateOrEditProjectReleaseDto ProjectRelease { get; set; }

        public string ProjectEnvironmentName { get; set; }

        public bool IsEditMode => ProjectRelease.Id.HasValue;
        public bool IsViewMode = false;

    }
}