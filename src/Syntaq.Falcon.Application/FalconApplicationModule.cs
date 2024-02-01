using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Syntaq.Falcon.Authorization;

namespace Syntaq.Falcon
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(FalconApplicationSharedModule),
        typeof(FalconCoreModule)
        )]
    public class FalconApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FalconApplicationModule).GetAssembly());
        }
    }
}