using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Records;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records
{
    [Table("SfaRecordMatterAudits")]
    public class RecordMatterAudit : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual RecordMatterStatus Status { get; set; }

        [StringLength(RecordMatterAuditConsts.MaxDataLength, MinimumLength = RecordMatterAuditConsts.MinDataLength)]
        public virtual string Data { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

        public virtual Guid? RecordMatterId { get; set; }

        [ForeignKey("RecordMatterId")]
        public RecordMatter RecordMatterFk { get; set; }

    }
}