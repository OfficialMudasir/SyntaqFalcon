using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.DashboardCustomization;
using Syntaq.Falcon.Web.Areas.Falcon.Startup;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Web.DashboardCustomization;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class DashboardController : CustomizableDashboardControllerBase
    {
        public DashboardController(DashboardViewConfiguration dashboardViewConfiguration,
            IDashboardCustomizationAppService dashboardCustomizationAppService)
            : base(dashboardViewConfiguration, dashboardCustomizationAppService)
        {

        }

        //public ActionResult Index()
        //{
        //    return View();
        //}
        public async Task<ActionResult> Index()
        {
            return await GetView(FalconDashboardCustomizationConsts.DashboardNames.DefaultTenantDashboard);
        }
    }
}