using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.Submissions
{
	[Table("SfaSubmissions")]
	[Abp.Auditing.Audited]
	public class Submission : FullAuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }
		public virtual string AccessToken { get; set; }
		public virtual bool RequiresPayment { get; set; }
		public virtual string PaymentStatus { get; set; }
		public virtual decimal? PaymentAmount { get; set; }
		//public virtual long? VoucherId { get; set; }
		//public Voucher Voucher { get; set; }
		public virtual decimal? VoucherAmount { get; set; }
		public virtual decimal? AmountPaid { get; set; }
		public virtual string ChargeId { get; set; }
		[Required]
		public virtual string SubmissionStatus { get; set; }

        public virtual string Description { get; set; }

        public virtual string Type { get; set; }
		public virtual Guid? RecordId { get; set; }
		[ForeignKey("RecordId")]
		public Record RecordFk { get; set; }
		
		public virtual Guid? RecordMatterId { get; set; }
		[ForeignKey("RecordMatterId")]
		public RecordMatter RecordMatterFk { get; set; }

		public virtual long? UserId { get; set; }
		[ForeignKey("UserId")]
		public User UserFk { get; set; }

		public virtual Guid? AppId { get; set; }
		[ForeignKey("AppId")]
		public Apps.App AppFk { get; set; }

		public virtual Guid? AppJobId { get; set; }
		[ForeignKey("AppJobId")]
		public AppJob AppJobFk { get; set; }

		public virtual Guid? FormId { get; set; }
		[ForeignKey("FormId")]
		public Form FormFk { get; set; }
	}
}