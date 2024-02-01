using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectDeploymentForViewDto
    {
        public ProjectDeploymentDto ProjectDeployment { get; set; }

        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ProjectReleaseId { get; set; }
        public string ProjectReleaseName { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public int VersionRevision { get; set; }

        public int? TenantId { get; set; }
        public string TenantName { get; set; }

    }
}