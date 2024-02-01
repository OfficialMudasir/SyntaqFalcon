using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Configuration;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.Localization;
using Syntaq.Falcon.UiCustomization;
using Syntaq.Falcon.UiCustomization.Dto;

namespace Syntaq.Falcon.Web.Views
{
    public abstract class FalconRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject] public IAbpSession AbpSession { get; set; }

        [RazorInject] public IUiThemeCustomizerFactory UiThemeCustomizerFactory { get; set; }

        [RazorInject] public IAppConfigurationAccessor AppConfigurationAccessor { get; set; }

        protected FalconRazorPage()
        {
            LocalizationSourceName = FalconConsts.LocalizationSourceName;
        }

        public async Task<UiCustomizationSettingsDto> GetTheme()
        {
            var themeCustomizer = await UiThemeCustomizerFactory.GetCurrentUiCustomizer();
            var settings = await themeCustomizer.GetUiSettings();
            return settings;
        }

        public async Task<string> GetContainerClass()
        {
            var theme = await GetTheme();
            if (theme.BaseSettings.Layout.LayoutType == "fluid")
            {
                return "container-fluid";
            }

            return theme.BaseSettings.Layout.LayoutType == "fluid-xxl" ? "container-xxl" : "container";
        }

        public async Task<string> GetLogoSkin()
        {
            var theme = await GetTheme();
            if (theme.IsTopMenuUsed || theme.IsTabMenuUsed)
            {
                return theme.BaseSettings.Layout.DarkMode ? "light" : "dark";
            }

            return theme.BaseSettings.Menu.AsideSkin;
        }
        
        public string GetMomentLocale()
        {
            if (CultureHelper.UsingLunarCalendar)
            {
                return "en";
            }

            var momentLocaleMapping = AppConfigurationAccessor.Configuration.GetSection("LocaleMappings:Moment").Get<List<LocaleMappingInfo>>();
            if (momentLocaleMapping == null)
            {
                return CultureInfo.CurrentUICulture.Name;
            }

            var mapping = momentLocaleMapping.FirstOrDefault(e => e.From == CultureInfo.CurrentUICulture.Name);
            if (mapping == null)
            {
                return CultureInfo.CurrentUICulture.Name;
            }

            return mapping.To;
        }
    }
}
