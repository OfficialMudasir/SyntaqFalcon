using Syntaq.Falcon.Users;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Records;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Syntaq.Falcon.MultiTenancy;

namespace Syntaq.Falcon.Users
{
	[Table("SfaUserAcceptances")]
	public class UserAcceptance : FullAuditedEntity<Guid> , IMayHaveTenant
    {
		public int? TenantId { get; set; }

		public virtual int AcceptedDocTemplateVersion { get; set; }

		public virtual Guid? UserAcceptanceTypeId { get; set; }
		
        [ForeignKey("UserAcceptanceTypeId")]
		public UserAcceptanceType UserAcceptanceTypeFk { get; set; }
		
		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
		public virtual Guid? RecordMatterContributorId { get; set; }
		
        [ForeignKey("RecordMatterContributorId")]
		public RecordMatterContributor RecordMatterContributorFk { get; set; }

		[ForeignKey("TenantId")]
		public Tenant TenantFk { get; set; }
	}
}