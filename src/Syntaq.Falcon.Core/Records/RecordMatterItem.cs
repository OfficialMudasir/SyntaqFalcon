using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Organizations;
using Newtonsoft.Json;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Submissions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.Records
{
	[Table("SfaRecordMatterItems")]
	[Abp.Auditing.Audited]
	public class RecordMatterItem : FullAuditedEntity<Guid>, IMayHaveTenant
	{
		public int? TenantId { get; set; }

		//[NotMapped]
		public bool HasDocument { get; set; }

        public virtual bool LockOnBuild { get; set; } = false;

        public virtual byte[] Document { get; set; }

		[StringLength(RecordMatterItemConsts.MaxDocumentNameLength, MinimumLength = RecordMatterItemConsts.MinDocumentNameLength)]
		public virtual string DocumentName { get; set; }

		public virtual Guid GroupId { get; set; }
				
		public virtual Guid RecordMatterId { get; set; }

		[JsonIgnore]
		[ForeignKey("RecordMatterId")]
		public RecordMatter RecordMatter { get; set; }

		public virtual long? UserId { get; set; }
		public User User { get; set; }

		public virtual long? OrganizationUnitId { get; set; }
		public OrganizationUnit OrganizationUnit { get; set; }

		public virtual Guid? FormId { get; set; }
		public virtual string FormURI { get; set; }

		public virtual string DocumentTemplateId { get; set; }

		public virtual string AllowedFormats { get; set; } = "P";

        public virtual string AllowWordAssignees    { get; set; }
        public virtual string AllowPdfAssignees     { get; set; }
        public virtual string AllowHtmlAssignees    { get; set; }

        [Required]
		public virtual string Status { get; set; }

		public virtual string ErrorDetails { get; set; }

		public virtual Guid? SubmissionId { get; set; }
		[ForeignKey("SubmissionId")]
		public Submission Submission { get; set; }

		public int Order { get; set; } //order
		//public int RepeatOrder { get; set; } //Repeatorder
	}
}