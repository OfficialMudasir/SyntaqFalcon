using Syntaq.Falcon.Tags;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Tags
{
    [Table("SfaTagEntityTypes")]
    public class TagEntityType : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual EntityType EntityType { get; set; }

        public virtual Guid? TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag TagFk { get; set; }

    }
}