using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Syntaq.Falcon.Folders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.Documents
{
	[Table("SfaDocumentTemplates")]
	[Abp.Auditing.Audited]
	public class Template : CreationAuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }
		
		public virtual Guid OriginalId { get; set; }
		
		[StringLength(TemplateConsts.MaxNameLength, MinimumLength = TemplateConsts.MinNameLength)]
		public virtual string Name { get; set; }

		[StringLength(TemplateConsts.MaxNameLength, MinimumLength = TemplateConsts.MinNameLength)]
		public virtual string DocumentName { get; set; }

		public virtual byte[] Document { get; set; }
		
		public virtual int Version { get; set; }
		
		public virtual int CurrentVersion { get; set; }
		
		[StringLength(TemplateConsts.MaxCommentsLength, MinimumLength = TemplateConsts.MinCommentsLength)]
		public virtual string Comments { get; set; }

		public virtual Guid FolderId { get; set; }
		public Folder Folder { get; set; }

        public bool LockToTenant { get; set; }
	}
}