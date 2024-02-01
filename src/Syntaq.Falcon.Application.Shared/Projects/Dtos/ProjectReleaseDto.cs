using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class ProjectReleaseDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Notes { get; set; }

        public Guid ProjectId { get; set; }

        public bool Required { get; set; }

        public int VersionMajor { get; set; }

        public int VersionMinor { get; set; }

        public int VersionRevision { get; set; }

        public ProjectReleaseEnums.ProjectReleaseType ReleaseType { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

        public ProjectDto Project { get; set; }
        public   DateTime CreationTime { get; set; }

        public string TenantName { get; set; }
    }
}