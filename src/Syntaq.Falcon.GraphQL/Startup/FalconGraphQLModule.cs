using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Syntaq.Falcon.Startup
{
    [DependsOn(typeof(FalconCoreModule))]
    public class FalconGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FalconGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}