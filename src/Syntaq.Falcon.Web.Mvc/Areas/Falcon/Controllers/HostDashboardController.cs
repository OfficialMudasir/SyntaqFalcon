using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.DashboardCustomization;
using System.Threading.Tasks;
using Syntaq.Falcon.Web.Areas.Falcon.Startup;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
    public class HostDashboardController : CustomizableDashboardControllerBase
    {
        public HostDashboardController(
            DashboardViewConfiguration dashboardViewConfiguration,
            IDashboardCustomizationAppService dashboardCustomizationAppService)
            : base(dashboardViewConfiguration, dashboardCustomizationAppService)
        {

        }

        public async Task<ActionResult> Index()
        {
            return await GetView(FalconDashboardCustomizationConsts.DashboardNames.DefaultHostDashboard);
        }
    }
}