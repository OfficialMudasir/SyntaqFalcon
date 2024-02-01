using Syntaq.Falcon.Projects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Projects
{
    [Table("SfaProjectDeployments")]
    public class ProjectDeployment : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [StringLength(ProjectDeploymentConsts.MaxCommentsLength, MinimumLength = ProjectDeploymentConsts.MinCommentsLength)]
        public virtual string Comments { get; set; }

        public virtual ProjectDeploymentEnums.ProjectDeploymentActionType ActionType { get; set; }

        public virtual Guid? ProjectReleaseId { get; set; }

        [ForeignKey("ProjectReleaseId")]
        public ProjectRelease ProjectReleaseFk { get; set; }

        public bool Enabled { get; set; } = true;

    }
}