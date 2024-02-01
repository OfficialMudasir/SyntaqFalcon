using Syntaq.Falcon.Projects.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectEnvironments
{
    public class CreateOrEditProjectEnvironmentModalViewModel
    {
        public CreateOrEditProjectEnvironmentDto ProjectEnvironment { get; set; }

        public bool IsEditMode => ProjectEnvironment.Id.HasValue;
    }
}