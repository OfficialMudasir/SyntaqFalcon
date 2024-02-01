using System.Linq;
using System.Threading.Tasks;
using Abp.Localization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Views;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Components.FalconLanguageSwitch
{
    public class FalconLanguageSwitchViewComponent : FalconViewComponent
    {
        private readonly ILanguageManager _languageManager;

        public FalconLanguageSwitchViewComponent(ILanguageManager languageManager)
        {
            _languageManager = languageManager;
        }

        public Task<IViewComponentResult> InvokeAsync(string cssClass)
        {
            var model = new LanguageSwitchViewModel
            {
                Languages = _languageManager.GetActiveLanguages().ToList(),
                CurrentLanguage = _languageManager.CurrentLanguage,
                CssClass = cssClass
            };
            
            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
