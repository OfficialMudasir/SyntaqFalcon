using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Syntaq.Falcon.Configuration.Dto;
using Syntaq.Falcon.UiCustomization.Dto;

namespace Syntaq.Falcon.UiCustomization
{
    public interface IUiCustomizer : ISingletonDependency
    {
        Task<UiCustomizationSettingsDto> GetUiSettings();

        Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings);

        Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings, UserIdentifier changerUser);

        Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings, UserIdentifier changerUser);

        Task<ThemeSettingsDto> GetHostUiManagementSettings();

        Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId);

        Task UpdateDarkModeSettingsAsync(UserIdentifier user, bool isDarkModeEnabled);
    }
}