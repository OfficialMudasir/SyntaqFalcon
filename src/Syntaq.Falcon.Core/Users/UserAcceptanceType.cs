using Syntaq.Falcon.Documents;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Users
{
	[Table("SfaUserAcceptanceTypes")]
    public class UserAcceptanceType : CreationAuditedEntity<Guid> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[StringLength(UserAcceptanceTypeConsts.MaxNameLength, MinimumLength = UserAcceptanceTypeConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		public virtual bool Active { get; set; }
		

		public virtual Guid? TemplateId { get; set; }
		
        [ForeignKey("TemplateId")]
		public Template TemplateFk { get; set; }
		
    }
}