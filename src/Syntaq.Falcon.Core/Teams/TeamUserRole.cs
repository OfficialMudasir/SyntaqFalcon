
//using Abp.Domain.Entities;
//using Abp.Domain.Entities.Auditing;
//using Abp.Organizations;
//using Syntaq.Falcon.Authorization.Roles;
//using Syntaq.Falcon.Authorization.Users;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Syntaq.Falcon.Teams
//{
//    [Table("SfaTeamUserRoles")]
//    public class TeamUserRole : AuditedEntity<long> , IMayHaveTenant
//    {
//		public int? TenantId { get; set; }

//        public virtual int RoleId { get; set; }
//        //public Role Role { get; set; }

//		public virtual long OrganizationUnitId { get; set; }
//		//public OrganizationUnit OrganizationUnit { get; set; }
		
//		public virtual long UserId { get; set; }
//		//public User User { get; set; }
		
//    }
//}