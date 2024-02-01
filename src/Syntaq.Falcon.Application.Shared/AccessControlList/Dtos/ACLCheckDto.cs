using System;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
    public class ACLCheckDto
    {
        public string Action { get; set; }
        public Guid EntityId { get; set; }
        public long? UserId { get; set; }
        public long? TenantId { get; set; }
        public long? OrgId { get; set; }
        public string AccessToken { get; set; }
        public ACLCheckType Type { get; set; } = ACLCheckType.UserOrTeam;
    }

    public enum ACLCheckType
    {
        UserOrTeam = 1,
        Tenant = 2
    }

}
