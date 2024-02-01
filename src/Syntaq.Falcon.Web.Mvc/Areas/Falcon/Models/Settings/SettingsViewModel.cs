using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Configuration.Tenants.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Settings
{
    public class SettingsViewModel
    {
        public TenantSettingsEditDto Settings { get; set; }
        
        public List<ComboboxItemDto> TimezoneItems { get; set; }
        
        public List<string> EnabledSocialLoginSettings { get; set; } = new List<string>();
    }
}