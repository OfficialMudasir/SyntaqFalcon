using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.MultiTenancy;

namespace Syntaq.Falcon.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}