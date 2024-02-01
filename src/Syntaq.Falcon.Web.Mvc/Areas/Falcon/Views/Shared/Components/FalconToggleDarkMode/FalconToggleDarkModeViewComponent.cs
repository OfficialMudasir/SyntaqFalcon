using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Views;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Components.FalconToggleDarkMode
{
    public class FalconToggleDarkModeViewComponent : FalconViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, bool isDarkModeActive)
        {
            return Task.FromResult<IViewComponentResult>(View(new ToggleDarkModeViewModel(cssClass, isDarkModeActive)));
        }
    }
}