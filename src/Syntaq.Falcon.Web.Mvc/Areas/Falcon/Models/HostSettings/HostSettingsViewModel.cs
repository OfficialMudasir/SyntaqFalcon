using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Configuration.Host.Dto;
using Syntaq.Falcon.Editions.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.HostSettings
{
    public class HostSettingsViewModel
    {
        public HostSettingsEditDto Settings { get; set; }

        public List<SubscribableEditionComboboxItemDto> EditionItems { get; set; }

        public List<ComboboxItemDto> TimezoneItems { get; set; }

        public string IfHaveAnyActiveUserAcceptanceTypes { get; set; }

        public List<string> EnabledSocialLoginSettings { get; set; } = new List<string>();

    }
}