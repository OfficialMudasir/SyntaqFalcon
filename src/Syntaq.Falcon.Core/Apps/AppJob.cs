
using Syntaq.Falcon.Apps;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Apps
{
	[Table("SfaAppJobs")]
	[Abp.Auditing.Audited]
	public class AppJob : Entity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(AppJobConsts.MaxNameLength, MinimumLength = AppJobConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		public virtual string Data { get; set; }
		
		public virtual Guid AppId { get; set; }
		public virtual Guid? EntityId { get; set; }

		//public T ToObject<T>()
		//{
		//    throw new NotImplementedException();
		//}

		// public App App { get; set; }

	}
}