namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectDeployments
{
    public class ProjectDeploymentProjectReleaseLookupTableViewModel
    {
        public string ProjectEnvironmentId { get; set; }
        public string ProjectTemplateId { get; set; }
        public string ProjectId { get; set; }

        public string DisplayName { get; set; }

        public string FilterText { get; set; }
    }
}