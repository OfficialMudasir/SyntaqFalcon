using Abp.Application.Navigation;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Components.FalconMenu
{
    public class UserMenuItemViewModel
    {
        public UserMenuItem MenuItem { get; set; }

        public string CurrentPageName { get; set; }

        public int MenuItemIndex { get; set; }

        public int ItemDepth { get; set; }

        public bool RootLevel { get; set; }
        
        public bool IsTabMenuUsed { get; set; }
        
        public bool IconMenu { get; set; }
    }
}
