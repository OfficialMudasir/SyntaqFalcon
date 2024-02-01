using System.Threading.Tasks;

namespace Syntaq.Falcon.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
