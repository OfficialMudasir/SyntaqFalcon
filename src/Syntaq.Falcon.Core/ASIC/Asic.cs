using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Submissions;

namespace Syntaq.Falcon.ASIC
{
    [Table("SfaASICRequests")]
    [Abp.Auditing.Audited]
    public class Asic : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string HTTPRequests { get; set; }

        [Required]
        public AsicConsts.RequestMethod RequestMethod { get; set; }

        [Required]
        public virtual string Response { get; set; }

        public virtual string Data { get; set; }
        public virtual int? RequestId { get; set; } //should be changed as int
        //public virtual string RecordId { get; set; }


        public virtual Guid RecordId { get; set; }

        [ForeignKey("RecordId")]
        public Record Record { get; set; }

        public virtual Guid RecordMatterId { get; set; }

        //[ForeignKey("RecordMatterId")]
        //public RecordMatter RecordMatter { get; set; }

        public virtual Guid RecordMatterItemId { get; set; }

        //[ForeignKey("RecordMatterItemId")]
        //public RecordMatterItem RecordMatterItem { get; set; }

        public virtual Guid SubmissionId { get; set; }

        public virtual String AccessToken { get; set; }
        public virtual String Status { get; set; }
        public virtual Boolean ManualReview { get; set; } = false;
        public virtual Boolean Triggered { get; set; } = false;


    }
}