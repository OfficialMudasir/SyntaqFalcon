using Syntaq.Falcon.Projects.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectTenants
{
    public class CreateOrEditProjectTenantModalViewModel
    {
        public CreateOrEditProjectTenantDto ProjectTenant { get; set; }

        public string ProjectEnvironmentName { get; set; }

        public bool IsEditMode => ProjectTenant.Id.HasValue;
    }
}