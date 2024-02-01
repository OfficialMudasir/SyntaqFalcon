using Microsoft.AspNetCore.Authorization;

namespace Syntaq.Falcon.Authorization.Requirements
{
    public class ACLRoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; private set; }
        public string EntityKey { get; private set; }

        public ACLRoleRequirement(string role, string entitykey)
        {
            Role = role;
            EntityKey = entitykey;
        }
    }
}
