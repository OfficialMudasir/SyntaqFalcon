using Syntaq.Falcon.Projects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Projects
{
    [Table("SfaProjectTenants")]
    public class ProjectTenant : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual int SubscriberTenantId { get; set; }

        public virtual Guid ProjectId { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual Guid? ProjectEnvironmentId { get; set; }

        [ForeignKey("ProjectEnvironmentId")]
        public ProjectEnvironment ProjectEnvironmentFk { get; set; }

    }
}