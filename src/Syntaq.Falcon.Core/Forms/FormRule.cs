using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace Syntaq.Falcon.Forms
{
	[Table("SfaFormRules")]
	[Abp.Auditing.Audited]
	public class FormRule : AuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }			

		[Required]
		public virtual string Rule { get; set; }

		public virtual bool IsEnabled { get; set; } = true;	

		public virtual Guid FormId { get; set; }

		[JsonIgnore]
		public Form Form { get; set; }		
	}
}