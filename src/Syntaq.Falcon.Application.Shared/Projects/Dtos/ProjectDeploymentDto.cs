using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class ProjectDeploymentDto : EntityDto<Guid>
    {
        public string Comments { get; set; }

        public ProjectDeploymentEnums.ProjectDeploymentActionType ActionType { get; set; }

        public Guid? ProjectReleaseId { get; set; }

        public ProjectReleaseDto ProjectRelease {get;set;  }

        public DateTime CreationTime { get; set; }

        public bool Enabled { get; set; }
    }
}