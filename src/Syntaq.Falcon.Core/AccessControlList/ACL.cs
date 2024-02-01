using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Organizations;
using Syntaq.Falcon.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.AccessControlList
{
	[Audited]
	[Table("SfaACLs")]
	public class ACL : AuditedEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		public virtual Guid EntityID { get; set; }
		
		[Required]
		[StringLength(ACLConsts.MaxRoleLength, MinimumLength = ACLConsts.MinRoleLength)]
		public virtual string Role { get; set; }

		public virtual long? UserId { get; set; }
		public User User { get; set; }
		
		public virtual long? OrganizationUnitId { get; set; }
		public OrganizationUnit OrganizationUnit { get; set; }

		public virtual long? TargetTenantId { get; set; }

		//[ForeignKey("TargetTenantId")]
		//public Tenants.Tena TargetTenantFk { get; set; }

		public virtual bool Accepted { get; set; }

		[Required]
		public virtual string Type { get; set; }

        [NotMapped] // USed for anon access
        public String AccessToken { get; set; }
	}
}