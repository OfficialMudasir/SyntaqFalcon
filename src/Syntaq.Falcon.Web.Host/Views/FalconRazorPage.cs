using Abp.AspNetCore.Mvc.Views;

namespace Syntaq.Falcon.Web.Views
{
    public abstract class FalconRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected FalconRazorPage()
        {
            LocalizationSourceName = FalconConsts.LocalizationSourceName;
        }
    }
}
