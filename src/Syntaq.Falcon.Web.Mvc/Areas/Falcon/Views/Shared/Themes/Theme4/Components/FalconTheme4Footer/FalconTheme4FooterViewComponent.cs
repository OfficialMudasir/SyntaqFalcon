using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Session;
using Syntaq.Falcon.Web.Views;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Themes.Theme4.Components.FalconTheme4Footer
{
    public class FalconTheme4FooterViewComponent : FalconViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public FalconTheme4FooterViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(footerModel);
        }
    }
}
