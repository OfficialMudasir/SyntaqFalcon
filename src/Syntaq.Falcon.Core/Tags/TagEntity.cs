using Syntaq.Falcon.Tags;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Tags
{
    [Table("SfaTagEntities")]
    public class TagEntity : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual Guid EntityId { get; set; }

        public virtual EntityType EntityType { get; set; }

        public virtual Guid? TagValueId { get; set; }

        [ForeignKey("TagValueId")]
        public TagValue TagValueFk { get; set; }

    }
}