using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize]
    public class WidgetsController : FalconControllerBase
    {
        #region Host

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
        public IActionResult EditionStatistics()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/EditionStatistics");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
        public IActionResult HostTopStats()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/HostTopStats");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
        public IActionResult IncomeStatistics()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/IncomeStatistics");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
        public IActionResult RecentTenants()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/RecentTenants");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
        public IActionResult SubscriptionExpiringTenants()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/SubscriptionExpiringTenants");
        }
      
        #endregion

        #region Tenant

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DailySales()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DailySales");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult GeneralStats()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/GeneralStats");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult MemberActivity()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/MemberActivity");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProfitShare()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProfitShare");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult RegionalStats()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/RegionalStats");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult SalesSummary()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/SalesSummary");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult TopStats()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/TopStats");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult SubmissionLimit()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/SubmissionLimit");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult TodaySubmissions()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/TodaySubmissions");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult NewUsers()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/NewUsers");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult MostRecentSubmissions()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/MostRecentSubmissions");
        }
        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProjectStatus()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProjectStatus");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProjectCycle()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProjectCycle");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProjectStatusChart()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProjectStatusChart");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ActionRequiredChart()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ActionRequiredChart");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult WaitingContributor()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/WaitingContributor");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult RecentTemplates()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/RecentTemplates");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult WaitingOthers()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/WaitingOthers");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProjectTemplateUsageCount()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProjectTemplateUsageCount");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult ProjectRecentDocuments()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/ProjectRecentDocuments");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DocumentOnNew()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DocumentOnNew");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DocumentOnDraft()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DocumentOnDraft");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DocumentOnPublish()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DocumentOnPublish");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DocumentOnFinal()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DocumentOnFinal");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult DocumentOnAll()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/DocumentOnAll");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
        public IActionResult StartNewProject()
        {
            return PartialView("Components/CustomizableDashboard/Widgets/StartNewProject");
        }

       

        #endregion
    }
}
