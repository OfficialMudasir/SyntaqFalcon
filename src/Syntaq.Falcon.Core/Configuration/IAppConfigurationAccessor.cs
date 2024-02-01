using Microsoft.Extensions.Configuration;

namespace Syntaq.Falcon.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
