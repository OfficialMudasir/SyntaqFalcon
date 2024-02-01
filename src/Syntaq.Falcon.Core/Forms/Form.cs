using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Syntaq.Falcon.Folders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Syntaq.Falcon.Forms
{
	[Table("SfaForms")]
	[Abp.Auditing.Audited]
	public class Form : FullAuditedEntity<Guid>, IMayHaveTenant
	{
		public int? TenantId { get; set; }

		public virtual Guid OriginalId { get; set; }

		[StringLength(FormConsts.MaxNameLength, MinimumLength = FormConsts.MinNameLength)]
		public virtual string Name { get; set; }

		[StringLength(FormConsts.MaxDescriptionLength, MinimumLength = FormConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }

		public virtual string Schema { get; set; }
        public virtual string RulesSchema { get; set; }

        public virtual string Script { get; set; }

		public virtual string RulesScript { get; set; }

		public virtual int Version { get; set; }

		public virtual int CurrentVersion { get; set; }
		
		public virtual bool PaymentEnabled { get; set; }
		public virtual decimal PaymentAmount { get; set; }
		public virtual string PaymentCurrency { get; set; }
		public virtual string PaymentProcess { get; set; }
		public virtual string PaymentProvider { get; set; }
		public virtual string PaymentAccessToken { get; set; }
		public virtual string PaymentRefreshToken { get; set; }
		public virtual string PaymentPublishableToken { get; set; }

		public virtual Guid FolderId { get; set; }

		public Folder Folder { get; set; }

		public virtual int FormTypeId { get; set; }
		public FormType FormType { get; set; }

		[StringLength(FormConsts.MaxNameLength, MinimumLength = FormConsts.MinNameLength)]
		public string VersionName { get; set; }

		[NotMapped]
		public string ACLRole { get; set; }

		[NotMapped]
		public long? OrganizationUnitId { get; set; }

		public virtual bool IsEnabled { get; set; } = true;
        public virtual bool LockOnBuild { get; set; } = false;
        public bool LockToTenant { get; set; } = false;
        public bool RequireAuth { get; set; } = false;
        public virtual Guid? CustomCssId { get; set; } //If not implement custom css per form, remove this
	}
}