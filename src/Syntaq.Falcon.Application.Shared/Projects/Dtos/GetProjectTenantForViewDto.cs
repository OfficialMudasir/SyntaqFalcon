namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectTenantForViewDto
    {
        public ProjectTenantDto ProjectTenant { get; set; }

        public string ProjectEnvironmentName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
    }
}