using System.Globalization;

namespace Syntaq.Falcon.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}