using Syntaq.Falcon.Projects.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectDeployments
{
    public class CreateOrEditProjectDeploymentModalViewModel
    {
        public CreateOrEditProjectDeploymentDto ProjectDeployment { get; set; }

        public string ProjectReleaseName { get; set; }
        public string ProjectReleaseNotes { get; set; }

        public bool IsEditMode => ProjectDeployment.Id.HasValue;
    }
}