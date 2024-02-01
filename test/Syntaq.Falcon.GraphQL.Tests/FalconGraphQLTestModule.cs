using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Syntaq.Falcon.Configure;
using Syntaq.Falcon.Startup;
using Syntaq.Falcon.Test.Base;

namespace Syntaq.Falcon.GraphQL.Tests
{
    [DependsOn(
        typeof(FalconGraphQLModule),
        typeof(FalconTestBaseModule))]
    public class FalconGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FalconGraphQLTestModule).GetAssembly());
        }
    }
}