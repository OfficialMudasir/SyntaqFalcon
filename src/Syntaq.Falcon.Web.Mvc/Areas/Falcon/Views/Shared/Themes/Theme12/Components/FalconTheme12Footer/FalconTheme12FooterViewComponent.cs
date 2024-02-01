using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Session;
using Syntaq.Falcon.Web.Views;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Themes.Theme12.Components.FalconTheme12Footer
{
    public class FalconTheme12FooterViewComponent : FalconViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public FalconTheme12FooterViewComponent(IPerRequestSessionCache sessionCache)
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
