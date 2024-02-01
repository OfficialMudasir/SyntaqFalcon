using Syntaq.Falcon.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.EntityVersionHistories
{
    [Table("SfaEntityVersionHistories")]
    public class EntityVersionHistory : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(EntityVersionHistoryConsts.MaxNameLength, MinimumLength = EntityVersionHistoryConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public virtual string VersionName { get; set; }

        [StringLength(EntityVersionHistoryConsts.MaxDescriptionLength, MinimumLength = EntityVersionHistoryConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual int Version { get; set; }

        public virtual int PreviousVersion { get; set; }

        public virtual Guid EntityId { get; set; }
        public virtual string Data { get; set; }
        public virtual string PreviousData { get; set; }
        public virtual string NewData { get; set; }

        [Required]
        public virtual string Type { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

    }
}