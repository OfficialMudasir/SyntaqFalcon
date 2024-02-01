using Syntaq.Falcon.Tags;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Tags
{
    [Table("SfaTagValues")]
    public class TagValue : Entity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [StringLength(TagValueConsts.MaxValueLength, MinimumLength = TagValueConsts.MinValueLength)]
        public virtual string Value { get; set; }

        public virtual Guid? TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag TagFk { get; set; }

    }
}