using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Syntaq.Falcon.Configuration;

namespace Syntaq.Falcon.Test.Base.Configuration
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(
                typeof(FalconTestBaseModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }
    }
}
