using Syntaq.Falcon.Projects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Projects
{
    [Table("SfaProjectEnvironments")]
    public class ProjectEnvironment : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(ProjectEnvironmentConsts.MaxNameLength, MinimumLength = ProjectEnvironmentConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [StringLength(ProjectEnvironmentConsts.MaxDescriptionLength, MinimumLength = ProjectEnvironmentConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual ProjectEnvironmentConsts.ProjectEnvironmentType EnvironmentType { get; set; }

    }
}