using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Forms;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;
using static Syntaq.Falcon.Projects.ProjectConsts;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;

namespace Syntaq.Falcon.Records
{
	[Table("sfaRecordMatterContributors")]
    [Audited]
    public class RecordMatterContributor : FullAuditedEntity<Guid> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[StringLength(RecordMatterContributorConsts.MaxOrganizationNameLength, MinimumLength = RecordMatterContributorConsts.MinOrganizationNameLength)]
		public virtual string OrganizationName { get; set; }
		
		[StringLength(RecordMatterContributorConsts.MaxNameLength, MinimumLength = RecordMatterContributorConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		[StringLength(RecordMatterContributorConsts.MaxAccessTokenLength, MinimumLength = RecordMatterContributorConsts.MinAccessTokenLength)]
		public virtual string AccessToken { get; set; }
		
		public virtual DateTime Time { get; set; }

		public virtual RecordMatterContributorStatus Status { get; set; }

		public virtual ProjectStepStatus StepStatus { get; set; }
		
		public virtual ProjectStepRole StepRole { get; set; }
		
		public virtual ProjectStepAction StepAction { get; set; }
		
		public virtual bool Complete { get; set; }
		
		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public virtual string Email { get; set; }
		
		public virtual string FormSchema { get; set; }
		
		public virtual string FormScript { get; set; }
		
		public virtual string FormRules { get; set; }
		
		public virtual string FormPages { get; set; }
		

		public virtual Guid? RecordMatterId { get; set; }
		
        [ForeignKey("RecordMatterId")]
		public RecordMatter RecordMatterFk { get; set; }
		
		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
		public virtual Guid? FormId { get; set; }
		
        [ForeignKey("FormId")]
		public Form FormFk { get; set; }

		public bool Enabled { get; set; } = true;


		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailFrom { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailCC { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailBCC { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxSubjectLength, MinimumLength = RecordMatterContributorConsts.MinSubjectLength)]
		public string Subject { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxMessageLength, MinimumLength = RecordMatterContributorConsts.MinMessageLength)]
		public string Message { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxMessageLength, MinimumLength = RecordMatterContributorConsts.MinMessageLength)]
		public string Comments { get; set; }

	}
}