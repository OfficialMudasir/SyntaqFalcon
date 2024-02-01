using Syntaq.Falcon.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.Authorization.Users
{
    [Table("SfaUserPasswordHistories")]
    public class UserPasswordHistory : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(UserPasswordHistoryConsts.MaxPasswordHashLength, MinimumLength = UserPasswordHistoryConsts.MinPasswordHashLength)]
        public virtual string PasswordHash { get; set; }

        public virtual long UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

    }
}