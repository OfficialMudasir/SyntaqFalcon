using Syntaq.Falcon.Projects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Projects
{
    [Table("SfaProjectReleases")]
    public class ProjectRelease : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(ProjectReleaseConsts.MaxNameLength, MinimumLength = ProjectReleaseConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [StringLength(ProjectReleaseConsts.MaxNotesLength, MinimumLength = ProjectReleaseConsts.MinNotesLength)]
        public virtual string Notes { get; set; }

        public virtual byte[] Artifact { get; set; }

        public virtual Guid ProjectId { get; set; }

        public virtual bool Required { get; set; }

        public virtual int VersionMajor { get; set; }

        public virtual int VersionMinor { get; set; }

        public virtual int VersionRevision { get; set; }

        public virtual ProjectReleaseEnums.ProjectReleaseType ReleaseType { get; set; }

        public virtual Guid? ProjectEnvironmentId { get; set; }

        [ForeignKey("ProjectEnvironmentId")]
        public ProjectEnvironment ProjectEnvironmentFk { get; set; }

    }
}