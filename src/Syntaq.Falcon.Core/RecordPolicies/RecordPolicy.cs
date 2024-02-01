using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.RecordPolicies
{
    [Table("SfaRecordPolicies")]
    [Abp.Auditing.Audited]
    public class RecordPolicy : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(RecordPolicyConsts.MaxNameLength, MinimumLength = RecordPolicyConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public virtual int AppliedTenantId { get; set; }

    }
}