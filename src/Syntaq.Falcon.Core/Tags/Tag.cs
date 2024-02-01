using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace Syntaq.Falcon.Tags
{
    [Table("SfaTags")]
    public class Tag : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [StringLength(TagConsts.MaxNameLength, MinimumLength = TagConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public List<TagValue> Values { get; set; } = new List<TagValue>();

    }
}