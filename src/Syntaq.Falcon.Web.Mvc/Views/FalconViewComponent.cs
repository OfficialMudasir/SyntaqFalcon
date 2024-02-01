using Abp.AspNetCore.Mvc.ViewComponents;

namespace Syntaq.Falcon.Web.Views
{
    public abstract class FalconViewComponent : AbpViewComponent
    {
        protected FalconViewComponent()
        {
            LocalizationSourceName = FalconConsts.LocalizationSourceName;
        }
    }
}