using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Syntaq.Falcon
{
    public class FalconCoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FalconCoreSharedModule).GetAssembly());
        }
    }
}