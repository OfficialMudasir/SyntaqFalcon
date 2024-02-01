using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Syntaq.Falcon
{
    [DependsOn(typeof(FalconCoreSharedModule))]
    public class FalconApplicationSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FalconApplicationSharedModule).GetAssembly());
        }
    }
}