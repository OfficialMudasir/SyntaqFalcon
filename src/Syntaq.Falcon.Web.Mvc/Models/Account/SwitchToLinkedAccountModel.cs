using System.ComponentModel.DataAnnotations;
using Abp;

namespace Syntaq.Falcon.Web.Models.Account
{
    public class SwitchToLinkedAccountModel
    {
        public int? TargetTenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long TargetUserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TargetTenantId, TargetUserId);
        }
    }
}