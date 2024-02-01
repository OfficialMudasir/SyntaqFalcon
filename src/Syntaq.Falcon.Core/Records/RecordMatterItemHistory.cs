using Syntaq.Falcon.Records;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Submissions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Records
{
    [Table("sfaRecordMatterItemHistories")]
    public class RecordMatterItemHistory : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxDocumentNameLength, MinimumLength = RecordMatterItemHistoryConsts.MinDocumentNameLength)]
        public virtual string DocumentName { get; set; }

        public virtual byte[] Document { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxNameLength, MinimumLength = RecordMatterItemHistoryConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public virtual Guid GroupId { get; set; }
        public virtual Guid? ContributorId { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxAllowedFormatsLength, MinimumLength = RecordMatterItemHistoryConsts.MinAllowedFormatsLength)]
        public virtual string AllowedFormats { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxStatusLength, MinimumLength = RecordMatterItemHistoryConsts.MinStatusLength)]
        public virtual string Status { get; set; }

        public virtual Guid? RecordMatterItemId { get; set; }

        [ForeignKey("RecordMatterItemId")]
        public RecordMatterItem RecordMatterItemFk { get; set; }

        public virtual Guid? FormId { get; set; }

        [ForeignKey("FormId")]
        public Form FormFk { get; set; }

        public virtual Guid? SubmissionId { get; set; }

        [ForeignKey("SubmissionId")]
        public Submission SubmissionFk { get; set; }

    }
}