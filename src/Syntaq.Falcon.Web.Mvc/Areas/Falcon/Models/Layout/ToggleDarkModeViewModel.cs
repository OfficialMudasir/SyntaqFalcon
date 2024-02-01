namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Layout
{

    public class ToggleDarkModeViewModel
    {
        public string CssClass { get; }
        public bool IsDarkModeActive { get; }

        public ToggleDarkModeViewModel(string cssClass, bool isDarkModeActive)
        {
            CssClass = cssClass;
            IsDarkModeActive = isDarkModeActive;
        }
    }
}