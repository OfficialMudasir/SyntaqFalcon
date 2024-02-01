using Syntaq.Falcon.RecordPolicies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;

namespace Syntaq.Falcon.RecordPolicyActions
{
    [Table("RecordPolicyActions")]
    [Abp.Auditing.Audited]
    public class RecordPolicyAction : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(RecordPolicyActionConsts.MaxNameLength, MinimumLength = RecordPolicyActionConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public virtual int AppliedTenantId { get; set; }

        public virtual int ExpireDays { get; set; }

        public virtual bool Active { get; set; }

        public virtual RecordPolicyActionType Type { get; set; }

        public virtual RecordStatusType RecordStatus { get; set; }

        public virtual Guid RecordPolicyId { get; set; }

        [ForeignKey("RecordPolicyId")]
        public RecordPolicy RecordPolicyFk { get; set; }

    }
}