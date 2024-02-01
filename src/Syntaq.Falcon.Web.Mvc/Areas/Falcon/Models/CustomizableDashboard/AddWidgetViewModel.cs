using System.Collections.Generic;
using Syntaq.Falcon.DashboardCustomization.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.CustomizableDashboard
{
    public class AddWidgetViewModel
    {
        public List<WidgetOutput> Widgets { get; set; }

        public string DashboardName { get; set; }

        public string PageId { get; set; }
    }
}
