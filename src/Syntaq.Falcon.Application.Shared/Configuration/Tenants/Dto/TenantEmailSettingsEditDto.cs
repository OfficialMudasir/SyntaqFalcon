using Abp.Auditing;
using Syntaq.Falcon.Configuration.Dto;

namespace Syntaq.Falcon.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}