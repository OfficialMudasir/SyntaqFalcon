using System.Collections.Generic;
using Abp.Localization;
using Syntaq.Falcon.Install.Dto;

namespace Syntaq.Falcon.Web.Models.Install
{
    public class InstallViewModel
    {
        public List<ApplicationLanguage> Languages { get; set; }

        public AppSettingsJsonDto AppSettingsJson { get; set; }
    }
}
