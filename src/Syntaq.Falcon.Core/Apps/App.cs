using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Apps
{
	[Table("SfaApps")]
	[Abp.Auditing.Audited]
	public class App : AuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(AppConsts.MaxNameLength, MinimumLength = AppConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		[StringLength(AppConsts.MaxDescriptionLength, MinimumLength = AppConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		
		[Required]
		public virtual string Data { get; set; }

        public virtual string RulesSchema { get; set; }

    }
}