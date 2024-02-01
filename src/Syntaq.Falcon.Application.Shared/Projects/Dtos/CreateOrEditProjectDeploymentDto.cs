using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class CreateOrEditProjectDeploymentDto : EntityDto<Guid?>
    {

        [StringLength(ProjectDeploymentConsts.MaxCommentsLength, MinimumLength = ProjectDeploymentConsts.MinCommentsLength)]
        public string Comments { get; set; }

        public ProjectDeploymentEnums.ProjectDeploymentActionType ActionType { get; set; }

        public Guid? ProjectReleaseId { get; set; }

        public String TenantName { get; set; }

    }
}